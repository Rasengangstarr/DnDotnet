open System

let mutable combatLog : string list = []

let log x =
    combatLog <- List.append combatLog [x]
    0

let objrandom = new Random()

let max (x,y) = if x > y then x else y

let StatToMod x = float ((x - 10) / 2) |> floor |> int

let ChalToProf(x : float) =
    if x <= 4.0 then 2 else
    if x <= 8.0 then 3 else
    if x <= 12.0 then 4 else
    if x <= 16.0 then 5 else
    if x <= 20.0 then 6 else
    if x <= 24.0 then 7 else
    if x <= 28.0 then 8 else
    if x <= 30.0 then 9 else
    0

//#region types

type Details = 
    { Name : string
      Description : string }


let Finesse x y = max(x,y)

let roll dice =
    let result = objrandom.Next(1,dice)
    log (sprintf "rolled %i on a d%i" result dice) |> ignore
    result
    


type Dice =
    {
        Sides : int
    }

type WeaponProperties =
    {
        Finesse : bool
        Ranged  : bool
    }

type ArmorClass =
    | Light = 0
    | Medium = 1
    | Heavy = 2

type Item = 
    | Weapon of details:Details * dice: int * properties: WeaponProperties
    | Armor of details:Details * modifier: int * armorClass: ArmorClass

type Creature = 
    {
        Details: Details
        Str : int
        Dex : int
        Con : int
        Int : int
        Wis : int
        Cha : int
        Items : Item list
        RightHand : Item
        Armor   : Item
        Level : float
        Hitpoints : int
    }

//#endregion

let EquipInRightHand item creature =
    if List.contains item creature.Items then {creature with RightHand = item} else creature 

let EquipInArmor item creature =
    if List.contains item creature.Items then {creature with Armor = item} else creature


let Give item creature =
    {creature with Items = List.append creature.Items [item]}

let ClampTo y x  =
    if x < y then x else y

let CalcToHit creature =
    let result : int = match creature.RightHand with
                       | Weapon(properties = p) ->
                        if p.Finesse then max(StatToMod creature.Str, StatToMod creature.Dex) + ChalToProf creature.Level else
                        if p.Ranged then StatToMod creature.Dex + ChalToProf creature.Level else
                        StatToMod creature.Str + ChalToProf creature.Level

    log (sprintf "%s gets an extra %i to their chance to hit" creature.Details.Name result) |> ignore
    result
              
let CalcAC creature =
    let result =
        match creature.Armor with
        | Armor(_, modifier, armorClass) ->
            modifier + match armorClass with
                       | ArmorClass.Light -> StatToMod creature.Dex
                       | ArmorClass.Medium -> StatToMod creature.Dex |> ClampTo 2
                       | ArmorClass.Heavy -> 0

    log (sprintf "%s has an AC of %i" creature.Details.Name result) |> ignore      
    result

let Hits creature target =
    CalcToHit creature + roll 20 > CalcAC target

let CalcDamage creature =
    let result =
        match creature.RightHand with
        | Weapon (dice = d) ->
            roll d
    log (sprintf "%s dealt %i" creature.Details.Name result) |> ignore
    result

let Attack creature target =
    log (sprintf "%s attacking %s" creature.Details.Name target.Details.Name) |> ignore
    if Hits creature target then CalcDamage creature else 0

let RemoveHitpoints creature x =
    let result = {creature with Hitpoints = creature.Hitpoints - x}
    log (sprintf "%s now has %i hitpoints" creature.Details.Name result.Hitpoints) |> ignore
    result

// #region records

let leatherArmor : Item =
    Armor ({ Name = "Leather Armor"; Description = "Stylish Leather Armor"; },
           13,
           ArmorClass.Light )

let chainMail : Item =
    Armor ({ Name = "Chain Mail"; Description = "Scary Chain Mail"; },
           16,
           ArmorClass.Heavy )

let noArmor : Item =
    Armor ({ Name = "No Armor"; Description = "Butt naked"; },
           0,
           ArmorClass.Light )

let scimitar : Item =
    Weapon ({ Name = "Scimitar"; Description = "A Scimitar"; },
            6,
            { Finesse = true; Ranged = false; })

let longsword : Item =
    Weapon ({ Name = "Longsword"; Description = "A Longsword"; },
            10,
            { Finesse = false; Ranged = false; })


let emptyHand : Item =
    Weapon ({ Name = "Empty Hand"; Description = "Bare fists"; },
            1,
            { Finesse = false; Ranged = false; })

let mutable goblin =
    {
        Details = { Name = "Goblin"; Description = "A mean looking goblin"};
        Str=8; Dex=14; Con=10; Int=10; Wis=10; Cha=9;
        Items = [];
        Armor = noArmor;
        Hitpoints = 7;
        RightHand = emptyHand;
        Level = 0.25;
    }

let mutable hobGoblin =
    {
        Details = { Name = "Hobgoblin"; Description = "A really mean looking goblin"};
        Str=13; Dex=12; Con=12; Int=10; Wis=10; Cha=9;
        Items = [];
        Armor = noArmor;
        Hitpoints = 11;
        RightHand = emptyHand;
        Level = 0.5;
    }

// #endregion

[<EntryPoint>]
let main argv =

    goblin <- Give scimitar goblin 
    |> Give leatherArmor 
    |> EquipInRightHand scimitar 
    |> EquipInArmor leatherArmor

    hobGoblin <- Give longsword hobGoblin 
    |> Give chainMail 
    |> EquipInRightHand longsword 
    |> EquipInArmor chainMail

    hobGoblin <- Attack goblin hobGoblin
    |> RemoveHitpoints hobGoblin

    goblin <- Attack hobGoblin goblin
    |> RemoveHitpoints goblin

    let mutable Break = false

    while not Break do
        if goblin.Hitpoints > 0 then
            hobGoblin <- Attack goblin hobGoblin
            |> RemoveHitpoints hobGoblin
        else
            for item in combatLog do
                printfn "%s" item
            printf "the goblin is dead!"
            Break <-true

        if hobGoblin.Hitpoints > 0 then
            goblin <- Attack hobGoblin goblin
            |> RemoveHitpoints goblin
        else 
            for item in combatLog do
                printfn "%s" item
            printf "the hobgoblin is dead!"
            Break <-true

    

    0
    