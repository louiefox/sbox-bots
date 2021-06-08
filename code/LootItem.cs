using Sandbox;
using System;
using System.Collections.Generic;
using BattleRoyale;

public class LootItem
{
    public static Dictionary<string, LootItem> Items = new()
    {
        { "dm_pistol", new( ItemType.Weapon, "Pistol", ItemRarity.Common, "weapons/rust_pistol/rust_pistol.vmdl_c" ) { WeaponClass = "dm_pistol" } },
        { "dm_shotgun", new( ItemType.Weapon, "Waterpipe Shotgun", ItemRarity.Uncommon, "weapons/rust_shotgun/rust_shotgun.vmdl_c" ) { WeaponClass = "dm_shotgun" } },
        { "dm_pumpshotgun", new( ItemType.Weapon, "Pump Shotgun", ItemRarity.Rare, "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl_c" ) { WeaponClass = "dm_pumpshotgun" } },
        { "dm_smg", new( ItemType.Weapon, "Custom SMG", ItemRarity.Epic, "weapons/rust_smg/rust_smg.vmdl_c" ) { WeaponClass = "dm_smg" } },
        { "dm_crossbow", new( ItemType.Weapon, "Crossbow", ItemRarity.Legendary, "weapons/rust_crossbow/rust_crossbow.vmdl_c" ) { WeaponClass = "dm_crossbow" } },

        { "armour_plate", new( ItemType.Consumable, "Armour Plate", ItemRarity.Rare, "models/bots/armour_plate/armour_plate.vmdl" ) { ItemID = "armour_plate", MaxStack = 5 } },

        { "ammo_pistol", new( ItemType.Ammo, "Pistol Ammo", ItemRarity.Common, "models/rust_props/small_junk/carton_box.vmdl" ) { ItemID = "ammo_pistol", MaxStack = 60, SpawnAmount = 30 } },
        { "ammo_shotgun", new( ItemType.Ammo, "Shotgun Ammo", ItemRarity.Common, "models/rust_props/small_junk/carton_box.vmdl" ) { ItemID = "ammo_shotgun", MaxStack = 24, SpawnAmount = 8 } },
        { "ammo_crossbow", new( ItemType.Ammo, "Crossbow Ammo", ItemRarity.Common, "models/rust_props/small_junk/carton_box.vmdl" ) { ItemID = "ammo_crossbow", MaxStack = 8, SpawnAmount = 4 } },
        { "ammo_rifle", new( ItemType.Ammo, "Rifle Ammo", ItemRarity.Common, "models/rust_props/small_junk/carton_box.vmdl" ) { ItemID = "ammo_rifle", MaxStack = 60, SpawnAmount = 30 } },
    };

    public ItemType Type;
    public string Name;
    public ItemRarity Rarity;
    public string Model;

    public string WeaponClass;
    public string ItemID;
    public int MaxStack = 1;
    public int SpawnAmount = 1;

    public LootItem( ItemType type, string name, ItemRarity rarity, string model )
    {
        Type = type;
        Name = name;
        Rarity = rarity;
        Model = model;
    }

    public int GiveItem( Player player, Vector3 pickupPos, int amount )
    {
        BRPlayer ply = player as BRPlayer;

        if ( Type == ItemType.Weapon )
        {
            BRWeaponInventory inventory = ply.WeaponInventory;

            int slot = inventory.Weapons.Count >= 2 ? inventory.CurrentSlot : 1;

            if ( inventory.Weapons.ContainsKey( slot ) )
            {
                inventory.Drop( slot, pickupPos );
            }

            return inventory.Add( slot, Library.Create<BaseBRWeapon>( WeaponClass ) ) ? 1 : 0;
        }
        else if ( Type == ItemType.Consumable || Type == ItemType.Ammo )
        {
            return ply.ItemInventory.Add( new BRInventoryItem( ItemID, amount ) );
        }


        return 0;
    }

    public bool CombineItem(LootPickup ent1, LootPickup ent2)
    {
        if ( (Type == ItemType.Consumable || Type == ItemType.Ammo) && ent1.Amount < MaxStack )
        {
            int change = Math.Min( MaxStack - ent1.Amount, ent2.Amount );
            ent1.Amount += change;
            ent2.Amount -= change;

            if( ent2.Amount <= 0 )
            {
                ent2.Delete();
            }

            return true;
        }

        return false;
    }
}

public enum ItemType
{
    Weapon,
    Consumable,
    Ammo
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
