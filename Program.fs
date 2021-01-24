open System

type Details =
    {
        Identifier : string
        Name : string
        Description : string
    }

type ArmorClass =
    | Light = 0
    | Medium = 1
    | Heavy = 2

type ItemPropertyExecutionPoint =
    | ToHitModCalc = 0
    | ToHitRollCalc = 1
    | DamageCalc = 2

type ItemPropertyCompositionStyle =
    | Base = 0
    | Additive = 1

type World =
    {
        Items : Item list
        Characters : Character list
    }

and Character =
    {
        Details: Details
        Strength : int
        Dexterity : int
        Inventory : Item list
        RightHandEquipped : Item
    }

and WeaponProperty =
    {
        ExecutionPoint : ItemPropertyExecutionPoint
        CompositionStyle : ItemPropertyCompositionStyle
        Function : Character -> int
    }
    member this.Execute =
        this.Function

and ItemKind = 
    | Weapon of diceDamage: int * properties: WeaponProperty list
    | Armor of modifier: int * armorClass: ArmorClass

and Item =
    {
        Details : Details
        Kind : ItemKind
    }

let max x y =
    if x > y then x else y 

let add x y =
    x + y

let applyBaseModifiers creature modifiers =
    modifiers
    |> List.filter (fun n -> n.CompositionStyle = ItemPropertyCompositionStyle.Base)
    |> List.map (fun n -> n.Execute creature) 
    |> List.reduce max

let applyAdditiveModifiers creature modifiers =
    modifiers
    |> List.filter (fun n -> n.CompositionStyle = ItemPropertyCompositionStyle.Additive)
    |> List.map (fun n -> n.Execute creature) 
    |> List.reduce add

let applyModifiers creature modifiers =
    applyBaseModifiers creature modifiers + applyAdditiveModifiers creature modifiers

let getToHitModifier creature weapon =
    match creature.RightHandEquipped.Kind with
    | Weapon(_, properties) ->
        properties 
        |> applyModifiers creature
    | _ ->
        0

let finesse =
    {
        ExecutionPoint = ItemPropertyExecutionPoint.ToHitModCalc;
        CompositionStyle = ItemPropertyCompositionStyle.Base;
        Function = fun x -> max x.Dexterity x.Strength;
    }

let scimitar =
    {
        Details = { Identifier = "weapon_scimitar"; Name = "Scimitar"; Description = "A curved sword" };
        Kind = Weapon(6, [finesse]);
    }

let goblin =
    {
        Details = { Identifier = "creature_goblin_1"; Name = "Goblin"; Description = "A Scary Goblin" };
        Strength = 8;
        Dexterity = 14;
        Inventory = [scimitar];
        RightHandEquipped = scimitar;
    }

let world =
    {
        Characters = [goblin];
        Items = [scimitar];
    }

[<EntryPoint>]
let main argv =
    
    0