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
    objrandom.Next(1,dice)

let max (x,y) =
    if x > y then x else y

type Armor(ac) =
    member this.AC = ac

type Weapon(damageDice, damageMod, isFinesse, name) =
    member this.DamageDice : int = damageDice
    member this.DamageMod : int = damageMod
    member this.IsFinesse : bool = isFinesse
    member this.Name : string = name

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
        printf "the %s is attacking the %s with a %s \n" this.Name target.Name this.Weapon.Name
        printf "the %s needs %i to hit... \n" this.Name target.Armor.AC
        let toHit =
            if isFinesse 
            then max(this.StrMod, this.DexMod) + ChalToProf this.CR 
            else this.StrMod + ChalToProf this.CR

        printf "the %s get plus %i to hit (%i from stats, %i from challenge level) \n" this.Name toHit (max(this.StrMod, this.DexMod)) (ChalToProf this.CR)
        let hitroll = roll 20 
        if toHit + hitroll > target.Armor.AC
        then printf "he rolls %i, hitting and causing %i damage \n" hitroll this.Weapon.Damage
        else printf "he rolls %i, he misses \n" hitroll

[<EntryPoint>]
let main argv =
    let goblin = new Character(new StatVect(8,14,10,10,8,8), "goblin", "Small humanoid (goblinoid), Neutral Evil", new Armor(15), new Weapon(6,2,true, "scimitar"), 0.25)
    let hobgoblin = new Character(new StatVect(13,12,12,10,10,9), "hobgoblin", "Medium humanoid (goblinoid), Lawful Evil", new Armor(18), new Weapon(10,1,false, "long sword"), 0.5)
    
    goblin.Attack hobgoblin
    hobgoblin.Attack goblin
    // let message = from "F#" // Call the function
    // printfn "Hello world %s" message
    0 // return an integer exit code
    