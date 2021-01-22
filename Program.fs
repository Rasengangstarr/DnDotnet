// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom


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

type Character(stats : StatVect, name, description) =
    member this.Stats = stats
    member this.Name = name
    member this.Description = description

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


[<EntryPoint>]
let main argv =
    let goblin = new Character(new StatVect(8,14,10,10,8,8), "goblin", "Small humanoid (goblinoid), Neutral Evil")
    printfn "%i" goblin.StrMod
    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // return an integer exit code
    