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

	public bool Add( int slot, Entity ent, bool select )
	{
		var weapon = ent as BaseBRWeapon;

		if ( weapon == null || Weapons.Count >= 2 )
		{
			ent.Delete();
			return false;
		}

		Weapons[slot] = ent;

		ent.Parent = Owner;
		ent.OnCarryStart( Owner );

		if ( select )
		{
			CurrentSlot = slot;
			Owner.ActiveChild = Weapons[slot];
		}

		return true;
	}

	public bool Add( int slot, Entity ent )
	{
		return Add( slot, ent, false );
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
