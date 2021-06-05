using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

partial class BRWeaponInventory
{
	public Entity Owner { get; init; }
	public IDictionary<int, Entity> Weapons = new Dictionary<int, Entity>();
	public int CurrentSlot = 0;

	public BRWeaponInventory( Entity owner )
	{
		Owner = owner;
	}

	public bool Add( int slot, Entity ent )
	{
		if ( ent as BaseBRWeapon == null )
		{
			ent.Delete();
			return false;
		}

		if ( Weapons.ContainsKey( slot ) ) return false;

		Weapons[slot] = ent;

		ent.Parent = Owner;
		ent.OnCarryStart( Owner );

		CurrentSlot = slot;
		Owner.ActiveChild = ent;

		return true;
	}

	public bool Remove( int slot, bool delete )
	{
		if ( !Weapons.ContainsKey( slot ) ) return false;

		if ( delete && Weapons[slot].IsValid() )
		{
			Weapons[slot].Delete();
		}

		Weapons.Remove( slot );

		return true;
	}	
	
	public Entity Drop( int slot, Vector3 pos )
	{
		if ( !Weapons.ContainsKey( slot ) ) return null;

		Entity ent = Weapons[slot];
		Remove( slot, false );

		LootPickup lootEnt = new LootPickup
		{
			Position = pos
		};

		lootEnt.SetItem( ent.ClassInfo.Name );

		return lootEnt;
	}

	public Entity Drop( int slot )
	{
		return Drop( slot, Owner.Position + Owner.Rotation.Forward * 60 );
	}

	public void SelectNext()
	{
		if( Weapons.ContainsKey( CurrentSlot+1 ) )
		{
			CurrentSlot++;
		} else
		{
			CurrentSlot = 0;
		}

		if ( !Weapons.ContainsKey( CurrentSlot ) ) return;
		Owner.ActiveChild = Weapons[CurrentSlot];
	}
}
