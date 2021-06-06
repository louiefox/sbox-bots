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

        { "armour_plate", new( ItemType.Consumable, "Armour Plate", ItemRarity.Rare, "models/rust_props/small_junk/carton_box.vmdl_c" ) { ItemID = "armour_plate" } },

        { "ammo_pistol", new( ItemType.Ammo, "Pistol Ammo", ItemRarity.Common, "models/rust_props/small_junk/carton_box.vmdl_c" ) { ItemID = "ammo_pistol" } },
    };

    public ItemType Type;
    public string Name;
    public ItemRarity Rarity;
    public string Model;

    public string WeaponClass;
    public string ItemID;

    public LootItem( ItemType type, string name, ItemRarity rarity, string model )
    {
        Type = type;
        Name = name;
        Rarity = rarity;
        Model = model;
    }

    public bool GiveItem( Player player, Vector3 pickupPos )
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

            return inventory.Add( slot, Library.Create<BaseBRWeapon>( WeaponClass ) );
        }
        else if ( Type == ItemType.Consumable || Type == ItemType.Ammo )
        {
            return ply.ItemInventory.Add( new BRInventoryItem( ItemID, 1 ) );
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
