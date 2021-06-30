using Sandbox;
using System;
using System.Collections.Generic;

[Library( "ent_supplycrate" )]
public partial class SupplyCrate : FloorUsable
{
	[Net]
	public bool Looted { get; set; } = false;

    public bool AllowDamage = false;
    public static List<(Vector3, Angles)> SpawnLocations = new()
    {
        (new Vector3( -4733, 2368, 619 ), new Angles( 0, 134, 0 )),
        (new Vector3( -4922, 2558, 619 ), new Angles( 0, -6, 0 )),
        (new Vector3( -3113, 4235, 412 ), new Angles( 0, -169, 0 )),
        (new Vector3( -3496, 4581, 425 ), new Angles( 0, -133, 0 )),
        (new Vector3( -3623, 4423, 427 ), new Angles( 0, -130, 0 )),
        (new Vector3( -4924, 4929, 182 ), new Angles( 0, -71, 0 )),
        (new Vector3( -7617, 7327, 82 ), new Angles( 0, -176, 0 )),
        (new Vector3( -7066, 6548, 331 ), new Angles( 0, 166, 0 )),
        (new Vector3( -8136, 1271, 161 ), new Angles( 0, 40, 0 )),
        (new Vector3( -2344, 2307, 149 ), new Angles( 0, -54, 0 )),
        (new Vector3( -2385, 2434, 153 ), new Angles( 0, 118, 0 )),
        (new Vector3( -1817, 6024, 285 ), new Angles( 0, 98, 0 )),
        (new Vector3( -1411, 7703, 120 ), new Angles( 0, 52, 0 )),
    };

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen_props/crate01.vmdl_c" );
		SetupPhysicsFromModel( PhysicsMotionType.Static, false );

        CreateModelCL();

        GlowState = GlowStates.GlowStateOn;
		GlowDistanceStart = 0;
		GlowDistanceEnd = 1000;
		GlowColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		GlowActive = true;
	}

    [ClientRpc]
    public void CreateModelCL()
    {
        SetModel( "models/citizen_props/crate01.vmdl_c" );
        SetupPhysicsFromModel( PhysicsMotionType.Static, false );
    }

    public override void TakeDamage( DamageInfo info )
    {
        if ( !AllowDamage ) return;
        base.TakeDamage( info );
    }

    public override void Use( Player ply )
	{
		if ( ply.Position.Distance( Position ) > 200 ) return;

        Dictionary<string, float> weaponLoot = new Dictionary<string, float>()
		{
			{ "dm_shotgun", 40 },
			{ "dm_pumpshotgun", 25 },
			{ "dm_smg", 20 },
			{ "dm_crossbow", 15 },
		};        
        
        Dictionary<string, float> itemLoot = new Dictionary<string, float>()
		{
			{ "armour_plate", 30 },

			{ "ammo_pistol", 20 },
			{ "ammo_rifle", 20 },
			{ "ammo_shotgun", 20 },
			{ "ammo_crossbow", 10 },
		};

		Random random = new Random();
		for ( int i = 0; i < 3; i++ )
		{
			string itemID = "";
			double randomNum = random.NextDouble();
			double currentChance = 0f;
			foreach( var data in (i == 0 ? weaponLoot : itemLoot) )
			{
				currentChance += data.Value/100f;
				if ( randomNum <= currentChance )
				{
					itemID = data.Key;
					break;
				}
			}

			if( !LootItem.Items.ContainsKey( itemID ) || LootItem.Items[itemID] is not LootItem lootItem )
			{
				Log.Info( "BattleRoyale ERROR: Item ID generated from supply crate doesn't exist." );
				continue;
			}

			LootPickup lootEnt = new LootPickup();
            lootEnt.SetPosition( Position );
            lootEnt.SetItem( itemID, lootItem.SpawnAmount );
		}

        CreateParticleEffect();
        AllowDamage = true;
        TakeDamage( DamageInfo.Generic( 100 ) );

        Delete();
	}

    [ClientRpc]
    public void CreateParticleEffect()
    {
        var pickupEffect = Particles.Create( "particles/loot_pickup.vpcf" );
        pickupEffect.SetPosition( 0, Position );
    }
}
