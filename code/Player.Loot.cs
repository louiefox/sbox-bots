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
        if ( Local.Pawn == null || Local.Pawn.Camera is not BRThirdPersonCamera cam ) return null;

        var tr = Trace.Ray( cam.Pos, cam.Pos + cam.Rot.Forward * 250 )
            .Ignore( Local.Pawn )
            .Run();

        Vector3 startPos = tr.EndPos;

        FloorUsable newTarget = null;
        if ( (tr.Entity is FloorUsable || tr.Entity is LootPickupClientModel) && startPos.Distance( tr.Entity.Position ) <= 40 )
        {
            newTarget = (tr.Entity is FloorUsable ? tr.Entity : tr.Entity.Parent) as FloorUsable;
        }

        if ( newTarget == null )
        {
            newTarget = All.OfType<FloorUsable>().OrderBy( x => startPos.Distance( x.Position ) ).FirstOrDefault();
        }

        if ( newTarget == null || !newTarget.IsValid() || startPos.Distance( newTarget.Position ) > 40 ) return null;

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
