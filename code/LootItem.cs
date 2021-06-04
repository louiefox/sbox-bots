using Sandbox;
using System;
using System.Collections.Generic;

public class LootItem
{
	public static Dictionary<string, LootItem> Items = new Dictionary<string, LootItem>()
	{
		{ "dm_pistol", new LootItem( ItemType.Weapon, "Pistol", ItemRarity.Common, "weapons/rust_pistol/rust_pistol.vmdl_c", "dm_pistol" ) {} },
		{ "dm_shotgun", new LootItem( ItemType.Weapon, "Waterpipe Shotgun", ItemRarity.Uncommon, "weapons/rust_shotgun/rust_shotgun.vmdl_c", "dm_shotgun" ) {} },
		{ "dm_pumpshotgun", new LootItem( ItemType.Weapon, "Pump Shotgun", ItemRarity.Rare, "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl_c", "dm_shotgun" ) {} },
		{ "dm_smg", new LootItem( ItemType.Weapon, "Custom SMG", ItemRarity.Rare, "weapons/rust_smg/rust_smg.vmdl_c", "dm_smg" ) {} },
		{ "dm_crossbow", new LootItem( ItemType.Weapon, "Crossbow", ItemRarity.Epic, "weapons/rust_crossbow/rust_crossbow.vmdl_c", "dm_crossbow" ) {} },
    };

	public ItemType Type;
	public string Name;
	public ItemRarity Rarity;
	public string Model;
	public string WeaponClass;

	public LootItem( ItemType type, string name, ItemRarity rarity, string model, string wepClass )
	{
		Type = type;
		Name = name;
		Rarity = rarity;
		Model = model;
		WeaponClass = wepClass;
	}

	public void GiveItem( Player player, Vector3 pickupPos )
	{
		BRPlayer ply = player as BRPlayer;
		BRWeaponInventory inventory = ply.WeaponInventory;

		int slot = inventory.CurrentSlot;
		
		if( inventory.Weapons.ContainsKey( slot ) )
		{
			inventory.Drop( slot, pickupPos + new Vector3( 0, 0, 40f ) );
		}

		inventory.Add( slot, Library.Create<BaseBRWeapon>( WeaponClass ) );
	}
}

public enum ItemType
{
	Weapon
}

public enum ItemRarity
{
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary
}
