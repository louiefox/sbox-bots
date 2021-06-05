using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

partial class BRInventory
{
	public Entity Owner { get; init; }

	[Net, Local]
	public IDictionary<int, BRInventoryItem> Slots { get; set; } = new Dictionary<int, BRInventoryItem>();

	public int MaxSlots { get; set; } = 16;

	public BRInventory( Entity owner )
	{
		Owner = owner;
	}

	public bool Add( BRInventoryItem data, int slot )
	{
		if ( Slots.ContainsKey( slot ) || slot > MaxSlots ) return false;

		Slots[slot] = data;

		return true;
	}	
	
	public bool Add( BRInventoryItem data )
	{
		return Add( data, GetNewSlot() );
	}

	public int GetNewSlot()
	{
		int newIndex = 0;
		for ( int i = 1; i <= Slots.Count; i++ )
		{
			if ( Slots.ContainsKey( i ) ) continue;

			newIndex = i;
			break;
		}

		return newIndex >= 1 ? newIndex : Slots.Count+1;
	}

	public bool Remove( int slot )
	{
		if ( !Slots.ContainsKey( slot ) ) return false;

		Slots.Remove( slot );

		return true;
	}	
	
	public Entity Drop( int slot, Vector3 pos )
	{
		if ( !Slots.ContainsKey( slot ) ) return null;

		BRInventoryItem data = Slots[slot];
		Remove( slot );

		LootPickup lootEnt = new LootPickup
		{
			Position = pos
		};

		lootEnt.SetItem( data.ItemID );

		return lootEnt;
	}

	public Entity Drop( int slot )
	{
		return Drop( slot, Owner.Position + new Vector3( 0, 0, 20f ) );
	}
}

public struct BRInventoryItem
{
	public string ItemID;
	public int Amount;

	public BRInventoryItem( string itemID, int amount )
	{
		ItemID = itemID;
		Amount = amount;
	}
}
