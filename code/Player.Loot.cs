using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using BattleRoyale;

partial class BRPlayer
{
    private FloorUsable LootTarget;

    public static FloorUsable GetNewTargetLoot()
    {
        if ( Local.Pawn.Camera is not BRThirdPersonCamera cam ) return null;

        var tr = Trace.Ray( cam.Pos, cam.Pos + cam.Rot.Forward * 250 )
            .Ignore( Local.Pawn )
            .Run();

        Vector3 startPos = tr.EndPos;

        List<LootPickupDist> sortedEnts = new List<LootPickupDist>();
        foreach ( var ent in All )
        {
            if ( ent is FloorUsable lootEnt )
            {
                bool lookingAtClientModel = false;
                if ( ent is LootPickup pickupEnt && tr.Entity == pickupEnt.ClientModel )
                {
                    lookingAtClientModel = true;
                }

                float distance = startPos.Distance( lootEnt.Position );
                if ( distance > 30 && tr.Entity != ent && !lookingAtClientModel ) continue;

                sortedEnts.Add( new LootPickupDist( lootEnt as FloorUsable, distance ) );
            }
        }

        sortedEnts = sortedEnts.OrderBy( o => o.Distance ).ToList();

        if ( sortedEnts.Count <= 0 ) return null;

        FloorUsable newTarget = sortedEnts[0].Ent;

        if ( newTarget == null || !newTarget.IsValid() ) return null;

        return newTarget;
    }

    [Event( "client.tick" )]
    public void LootPickupNearest()
    {
        FloorUsable oldTarget = LootTarget;
        LootTarget = GetNewTargetLoot();

        if ( oldTarget == LootTarget ) return;

        if ( oldTarget != null && oldTarget.IsValid() )
        {
            oldTarget.DisableGlow();
        }

        if ( LootTarget == null || !LootTarget.IsValid() ) return;

        LootTarget.EnableGlow();
    }

    public void RequestLootPickup()
    {
        if ( !IsClient ) return;

        FloorUsable target = GetNewTargetLoot();

        if ( target == null ) return;

        ConsoleSystem.Run( "request_forusable_use", target.Index );
    }
}

public struct LootPickupDist
{
    public FloorUsable Ent;
    public float Distance;

    public LootPickupDist( FloorUsable ent, float distance )
    {
        Ent = ent;
        Distance = distance;
    }
}