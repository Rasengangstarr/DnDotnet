// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System

// let challengeRatings =
//     [4,2; 8,3; 12,4; 16,5; 20,6; 24,7; 28,8; 30,9;]

// let ChalToProf(x : float) =
//     if x <= 4.0 then 2 else
//     if x <= 8.0 then 3 else
//     if x <= 12.0 then 4 else
//     if x <= 16.0 then 5 else
//     if x <= 20.0 then 6 else
//     if x <= 24.0 then 7 else
//     if x <= 28.0 then 8 else
//     if x <= 30.0 then 9 else
//     0

// type WeaponTypes = Str | Dex | Fin

let objrandom = new Random()

// // Define a function to construct a message to print
// let from whom =
//     sprintf "from %s" whom

// let roll dice =
//     objrandom.Next(1,dice)

// let RollWithMod dice x =
//     roll dice + x



// let Damage dd dm h  =
//     if h then RollWithMod dd dm else 0

// type Armor(ac) =
//     member this.AC = ac

// type Weapon(damageDice, damageMod, wt : WeaponTypes, name) =
//     member this.DamageDice : int = damageDice
//     member this.DamageMod : int = damageMod
//     member this.Name : string = name



// type StatVect =
//     struct
//         val Str : int
//         val Dex : int
//         val Con : int
//         val Int : int
//         val Wis : int
//         val Cha : int

//         new (str, dex, con, int, wis, cha) =
//             {Str = str; Dex = dex; Con = con; Int = int; Wis = wis; Cha = cha}

// end

// let StatToMod x =
//     float ((x - 10) / 2) |> floor |> int

// let ToHit (f : WeaponTypes, s : StatVect, p : int) =
//     match f with
//     | Str -> s.Str + p
//     | Dex -> s.Dex + p
//     | Fin -> max(s.Str,s.Dex) + p
//     | _   -> 0

// let Mt0 x =
//     x > 0

// type Character(stats : StatVect, name, description, armor: Armor, weapon: Weapon, cr) =
//     member this.Stats = stats
//     member this.Name = name
//     member this.Description = description
//     member this.Armor = armor
//     member this.CR : float = cr
//     member this.Weapon : Weapon = weapon

//     member private this.StatToMod(x) =
//         let xf = float ((x - 10) / 2)
//         let xfl = floor xf
//         int xfl
    
//     member this.StrMod = this.StatToMod this.Stats.Str
//     member this.DexMod = this.StatToMod this.Stats.Dex
//     member this.ConMod = this.StatToMod this.Stats.Con
//     member this.IntMod = this.StatToMod this.Stats.Int
//     member this.WisMod = this.StatToMod this.Stats.Wis
//     member this.ChaMod = this.StatToMod this.Stats.Cha

//     // member this.Attack(target : Character) =
         
//     //     let weapon = this.Weapon
//     //     let isFinesse = weapon.IsFinesse
//     //     printf "the %s is attacking the %s with a %s \n" this.Name target.Name this.Weapon.Name
        

//     //     printf "the %s get plus %i to hit (%i from stats, %i from challenge level) \n" this.Name toHit (max(this.StrMod, this.DexMod)) (ChalToProf this.CR)


// type Stat =    
//     | Str of int
//     | Dex of int
//     | Con of int
//     | Int of int
//     | Wis of int
//     | Cha of int


// type Stats =
//     {
//         Str : Stat.Str
//         Dex : Stat.Dex
//         Con : Stat.Con
//         Int : Stat.Int
//         Wis : Stat.Wis
//         Cha : Stat.Cha
//     }


let max (x,y) = if x > y then x else y

type Details = 
    { Name : string
      Description : string }


let Finesse x y = max(x,y)

let roll dice =
    objrandom.Next(1,dice)


type Dice =
    {
        Sides : int
    }

type Item = 
    | Weapon of Details * Dice

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
    }

let Equip item creature =
    if List.contains item creature.Items then {creature with RightHand = item} else creature

let Give item creature =
    {creature with Items = List.append creature.Items [item]}

let ToHit creature =


let scimitar : Item =
    Weapon ({ Name = "Scimitar"; Description = "A Scimitar"; },
        { Sides = 6; })

let emptyHand : Item =
    Weapon ({ Name = "Empty Hand"; Description = "Bare fists"; },
        { Sides = 1; })

let mutable goblin =
    {
        Details = { Name = "Goblin"; Description = "A mean looking goblin"};
        Str=8; Dex=14; Con=10; Int=10; Wis=10; Cha=9;
        Items = []
        RightHand = emptyHand;
    }

[<EntryPoint>]
let main argv =

    let goblin = Give scimitar goblin |> Equip scimitar
    
    0
    