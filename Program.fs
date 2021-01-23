// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System

// let challengeRatings =
//     [4,2; 8,3; 12,4; 16,5; 20,6; 24,7; 28,8; 30,9;]

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

let objrandom = new Random()

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

let roll dice =
    objrandom.Next(0,dice)

let max (x,y) =
    if x > y then x else y

type Armor(ac) =
    member this.AC = ac

type Weapon(damageDice, damageMod, isFinesse) =
    member this.DamageDice : int = damageDice
    member this.DamageMod : int = damageMod
    member this.IsFinesse : bool = isFinesse

    member this.Damage = (roll this.DamageDice) + this.DamageMod


type StatVect =
    struct
        val Str : int
        val Dex : int
        val Con : int
        val Int : int
        val Wis : int
        val Cha : int

        new (str, dex, con, int, wis, cha) =
            {Str = str; Dex = dex; Con = con; Int = int; Wis = wis; Cha = cha}

end

type Character(stats : StatVect, name, description, armor: Armor, weapon: Weapon, cr) =
    member this.Stats = stats
    member this.Name = name
    member this.Description = description
    member this.Armor = armor
    member this.CR : float = cr
    member this.Weapon : Weapon = weapon

    member private this.StatToMod(x) =
        let xf = float ((x - 10) / 2)
        let xfl = floor xf
        int xfl
    
    member this.StrMod = this.StatToMod this.Stats.Str
    member this.DexMod = this.StatToMod this.Stats.Dex
    member this.ConMod = this.StatToMod this.Stats.Con
    member this.IntMod = this.StatToMod this.Stats.Int
    member this.WisMod = this.StatToMod this.Stats.Wis
    member this.ChaMod = this.StatToMod this.Stats.Cha

    member this.Attack(target : Character) =
        let weapon = this.Weapon
        let isFinesse = weapon.IsFinesse
        let toHit =
            if isFinesse 
            then max(this.Stats.Str, this.Stats.Dex) + ChalToProf this.CR 
            else this.Stats.Str + ChalToProf this.CR
        if toHit + roll 20 > target.Armor.AC
        then this.Weapon.Damage
        else 0

[<EntryPoint>]
let main argv =
    let goblin = new Character(new StatVect(8,14,10,10,8,8), "goblin", "Small humanoid (goblinoid), Neutral Evil", new Armor(15), new Weapon(6,2,true), 0.25)
    let hobgoblin = new Character(new StatVect(13,12,12,10,10,9), "hobgoblin", "Medium humanoid (goblinoid), Lawful Evil", new Armor(18), new Weapon(10,1,false), 0.5)
    
    printfn "%i" (goblin.Attack hobgoblin)
    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // return an integer exit code
    