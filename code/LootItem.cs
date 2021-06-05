using Sandbox;
using System;
using System.Collections.Generic;

public class LootItem
{
	public static Dictionary<string, LootItem> Items = new Dictionary<string, LootItem>()
	{
		{ "dm_pistol", new LootItem( ItemType.Weapon, "Pistol", ItemRarity.Common, "weapons/rust_pistol/rust_pistol.vmdl_c" ) { WeaponClass  = "dm_pistol" } },
		{ "dm_shotgun", new LootItem( ItemType.Weapon, "Waterpipe Shotgun", ItemRarity.Uncommon, "weapons/rust_shotgun/rust_shotgun.vmdl_c" ) { WeaponClass  = "dm_shotgun" } },
		{ "dm_pumpshotgun", new LootItem( ItemType.Weapon, "Pump Shotgun", ItemRarity.Rare, "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl_c" ) { WeaponClass  = "dm_pumpshotgun" } },
		{ "dm_smg", new LootItem( ItemType.Weapon, "Custom SMG", ItemRarity.Epic, "weapons/rust_smg/rust_smg.vmdl_c" ) { WeaponClass  = "dm_smg" } },
		{ "dm_crossbow", new LootItem( ItemType.Weapon, "Crossbow", ItemRarity.Legendary, "weapons/rust_crossbow/rust_crossbow.vmdl_c" ) { WeaponClass  = "dm_crossbow" } },

		{ "armour_plate", new LootItem( ItemType.Consumable, "Armour Plate", ItemRarity.Rare, "models/rust_props/small_junk/carton_box.vmdl_c" ) {} },

		{ "ammo_pistol", new LootItem( ItemType.Ammo, "Pistol Ammo", ItemRarity.Common, "models/rust_props/small_junk/carton_box.vmdl_c" ) {} },
    };

	public ItemType Type;
	public string Name;
	public ItemRarity Rarity;
	public string Model;

	public string WeaponClass;

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

		switch( Type )
		{
			case ItemType.Weapon:
				BRWeaponInventory inventory = ply.WeaponInventory;

				int slot = inventory.Weapons.Count >= 2 ? inventory.CurrentSlot : 1;

				if ( inventory.Weapons.ContainsKey( slot ) )
				{
					inventory.Drop( slot, pickupPos );
				}

				return inventory.Add( slot, Library.Create<BaseBRWeapon>( WeaponClass ) );
			case ItemType.Consumable:
				ply.Armour += 50;
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
