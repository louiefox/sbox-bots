using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using BattleRoyale;

partial class BRPlayer
{
	private LootPickup LootTarget;

	public LootPickup GetNewTargetLoot()
	{
		BRThirdPersonCamera cam = Local.Pawn.Camera as BRThirdPersonCamera;
		if ( cam == null ) return null;

		var tr = Trace.Ray( cam.Pos, cam.Pos + cam.Rot.Forward * 250 )
			.Ignore( Local.Pawn )
			.Run();

		Vector3 startPos = tr.EndPos;

		List<LootPickupDist> sortedEnts = new List<LootPickupDist>();
		foreach ( var ent in All )
		{
			if ( ent is LootPickup lootEnt )
			{
				float distance = startPos.Distance( lootEnt.Position );
				if ( distance > 30 ) continue;

				sortedEnts.Add( new LootPickupDist( lootEnt as LootPickup, distance ) );
			}
		}

		sortedEnts = sortedEnts.OrderBy( o => o.Distance ).ToList();

		if ( sortedEnts.Count <= 0 ) return null;

		LootPickup newTarget = sortedEnts[0].Ent;

		if ( newTarget == null || !newTarget.IsValid() ) return null;

		return newTarget;
	}

	[Event( "client.tick" )]
	public void LootPickupNearest()
	{
		LootPickup oldTarget = LootTarget;
		LootTarget = GetNewTargetLoot();

		if ( oldTarget == LootTarget ) return;

		if ( oldTarget != null && oldTarget.IsValid() )
		{
			oldTarget.DisableGlow();
		}

		if ( LootTarget == null ) return;

		LootTarget.EnableGlow();
	}

	public void RequestLootPickup()
	{
		if ( !IsClient ) return;

		LootPickup target = GetNewTargetLoot();

		if ( target == null ) return;

		Log.Info( target.Index.ToString() );
		ConsoleSystem.Run( "request_loot_pickup", target.Index );
	}
}

public struct LootPickupDist
{
	public LootPickup Ent { set; get; }
	public float Distance { set; get; }

	public LootPickupDist(LootPickup ent, float distance)
	{
		Ent = ent;
		Distance = distance;
	}
}
