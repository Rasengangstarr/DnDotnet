open System

let stat2Mod x =
    (x - 10) / 2
    |> float
    |> floor
    |> int

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
        LeftHandEquipped : Item
        ArmorEquipped : Item
    }
    member this.StrMod = stat2Mod this.Strength
    member this.DexMod = stat2Mod this.Dexterity

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
    | Armor of armorClass: ArmorClass

and Item =
    {
        Details : Details
        Kind : ItemKind
    }

let max x y =
    if x > y then x else y 

let add x y =
    x + y

let log m f =
    let r = f
    printf m r
    r

let roll s =
    let r = System.Random()
    r.Next(1,s)

let getModifiersForExecutionPoint t l =
    List.filter (fun n -> n.ExecutionPoint = t) l

let getModifiersForStyle t l =
    List.filter (fun n -> n.CompositionStyle = t) l

let aggregateModifiers creature modifiers s f =
    modifiers
    |> getModifiersForStyle s
    |> List.map (fun n -> n.Execute creature) 
    |> fun a -> match a.Length with
                | 0 -> 0
                | _ -> a 
                       |> List.reduce f

let applyAdditiveModifiers creature modifiers =
    aggregateModifiers creature modifiers ItemPropertyCompositionStyle.Additive add
    
let applyBaseModifiers creature modifiers =
    aggregateModifiers creature modifiers ItemPropertyCompositionStyle.Base max

let applyModifiers creature modifiers =
    applyBaseModifiers creature modifiers + applyAdditiveModifiers creature modifiers

let getModifier executionPoint creature = 
    match creature.RightHandEquipped.Kind with
    | Weapon(_, properties) -> 
        properties
        |> getModifiersForExecutionPoint executionPoint
        |> applyModifiers creature
    | _ ->
        0

let getDamageModifier creature =
    getModifier ItemPropertyExecutionPoint.DamageCalc creature

let getToHitModifier creature =
    getModifier ItemPropertyExecutionPoint.ToHitModCalc creature

let getToHit creature =
    log "rolled to hit: %i" (roll 20)
    + log " + %i \n" (getToHitModifier creature)

let getBaseDamage creature =
    match creature.RightHandEquipped.Kind with
    | Weapon(dice, _) -> 
        dice
        |> roll
    | _ ->
        0

let getDamage creature =
    log "rolled damage: %i" (getBaseDamage creature)
    + log " + %i \n" (getDamageModifier creature)

let attack creature =
    match getToHit creature with
    | 0 -> 0
    | _ -> getDamage creature

let finesseHit =
    {
        ExecutionPoint = ItemPropertyExecutionPoint.ToHitModCalc;
        CompositionStyle = ItemPropertyCompositionStyle.Base;
        Function = fun x -> max x.DexMod x.StrMod;
    }

let finesseDamage =
    {
        ExecutionPoint = ItemPropertyExecutionPoint.DamageCalc;
        CompositionStyle = ItemPropertyCompositionStyle.Base;
        Function = fun x -> max x.DexMod x.StrMod;
    }

let melee =
    {
        ExecutionPoint = ItemPropertyExecutionPoint.ToHitModCalc;
        CompositionStyle = ItemPropertyCompositionStyle.Base;
        Function = fun x -> x.StrMod;
    }

let scimitar =
    {
        Details = { Identifier = "weapon_scimitar"; Name = "Scimitar"; Description = "A curved sword" };
        Kind = Weapon(6, [finesseHit; finesseDamage; melee]);
    }

let leatherArmor =
    {
        Details = { Identifier = "armor_leather"; Name = "Leather Armor"; Description = "some leather armor" };
        Kind = Weapon(6, [finesseHit; melee]);
    }

let goblin =
    {
        Details = { Identifier = "creature_goblin_1"; Name = "Goblin"; Description = "A Scary Goblin" };
        Strength = 8;
        Dexterity = 14;
        Inventory = [scimitar];
        RightHandEquipped = scimitar;
        LeftHandEquipped = scimitar;
        ArmorEquipped = scimitar;
    }


[<EntryPoint>]
let main argv =
    goblin
    |> attack
    
    0