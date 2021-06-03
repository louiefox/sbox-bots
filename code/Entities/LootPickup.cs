using Sandbox;
using System;
using System.Collections.Generic;

[Library( "ent_loot_pickup" )]
public partial class LootPickup : Prop
{
	private static Dictionary<int, LootPickup> IndexEnts = new Dictionary<int, LootPickup>();

	[Net]
	public int Index { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl_c" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );

		if ( !IsServer ) return;

		int highestIndex = 0;
		foreach( var data in IndexEnts )
		{
			highestIndex = Math.Max( highestIndex, data.Key );
		}

		for( int i = 0; i <= highestIndex+1; i++ )
		{
			if( !IndexEnts.ContainsKey( i ) )
			{
				Index = i;
				break;
			}
		}

		IndexEnts.Add( Index, this );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		IndexEnts.Remove( Index );
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	[ServerCmd( "request_loot_pickup" )]
	public static void RequestPickup(int index)
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		if ( owner is BRPlayer ply && IndexEnts.ContainsKey( index ) )
		{
			LootPickup ent = IndexEnts[index];
			if ( ent == null || !ent.IsValid() ) return;

			var pickupEffect = Particles.Create( "particles/money_pickup.vpcf" );
			pickupEffect.SetPos( 0, ent.Position );

			ent.Delete();
		}
	}
}
