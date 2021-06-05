using Sandbox;
using System;
using System.Collections.Generic;

[Library( "ent_supplycrate" )]
public partial class SupplyCrate : FloorUsable
{
	[Net]
	public bool Looted { get; set; } = false;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen_props/crate01.vmdl_c" );
		SetupPhysicsFromModel( PhysicsMotionType.Static, false );

		GlowState = GlowStates.GlowStateOn;
		GlowDistanceStart = 0;
		GlowDistanceEnd = 1000;
		GlowColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		GlowActive = true;
	}

	public override void Use( Player ply )
	{
		if ( ply.Position.Distance( Position ) > 200 ) return;

		Dictionary<string, float> lootTable = new Dictionary<string, float>()
		{
			{ "dm_pumpshotgun", 15 },
			{ "dm_smg", 5 },
			{ "dm_pistol", 40 },
			{ "armour_plate", 40 },
		};

		Random random = new Random();
		for ( int i = 0; i < 1; i++ )
		{
			string itemID = "";
			double randomNum = random.NextDouble();
			double currentChance = 0f;
			foreach( var data in lootTable )
			{
				currentChance += data.Value/100f;
				if ( randomNum <= currentChance )
				{
					itemID = data.Key;
					break;
				}
			}

			if( !LootItem.Items.ContainsKey( itemID ) )
			{
				Log.Info( "BattleRoyale ERROR: Item ID generated from supply crate doesn't exist." );
				continue;
			}

			LootPickup lootEnt = new LootPickup();
			lootEnt.Position = Position + new Vector3( 0, 0, 20f );
			lootEnt.SetItem( itemID );
		}

		Delete();
	}
}
