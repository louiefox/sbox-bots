
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using BattleRoyale;

public class MiniMap : Panel
{
    private Panel MapContents;
    private float PixelsPerUnit = .1f;
    private float ContentSize = 200f - 10f;
    private Dictionary<Panel, MapMarker> Markers = new();
    private Dictionary<Entity, Panel> MarkedCrates = new();

    private Panel PlayerMarker;
    private Panel ZoneMarker;

    public MiniMap()
	{
        StyleSheet.Load( "/ui/MiniMap.scss" );

        Add.Label( "N", "north" );
        Add.Label( "E", "east" );
        Add.Label( "S", "south" );
        Add.Label( "W", "west" );

        MapContents = Add.Panel( "mapcontents" );
        PlayerMarker = MapContents.Add.Panel( "playermarker" );

        Markers.Add( MapContents.Add.Panel( "redzone" ), new MapMarker( (Game.Current as BRGame).ZoneCenterPos, 3000f, 3000f ) );

        ZoneMarker = MapContents.Add.Panel( "safezone" );
        Markers.Add( ZoneMarker, new MapMarker( (Game.Current as BRGame).ZoneCenterPos, 10f, 10f ) );
    }

    public override void Tick()
	{
        Entity currentPawn = Local.Pawn;
        if ( Local.Pawn == null || !Local.Pawn.IsValid() )
        {
            if ( Local.Client.Camera is not BRSpectateCamera camera ) return;
            currentPawn = camera.CurrentTarget;
        }

        if ( currentPawn == null || !currentPawn.IsValid() ) return;

        if( Markers.ContainsKey( ZoneMarker ) )
        {
            float size = ((Game.Current as BRGame).DeathZoneDistance() * 2) * PixelsPerUnit;
            Markers[ZoneMarker] = new MapMarker( (Game.Current as BRGame).ZoneCenterPos, size, size );
        }

        foreach ( Entity ent in Entity.All )
        {
            if ( ent is not SupplyCrate || MarkedCrates.ContainsKey( ent ) ) continue;

            Panel panel = MapContents.Add.Panel( "cratemarker" );

            MarkedCrates.Add( ent, panel );
            Markers.Add( panel, new MapMarker( ent.Position, 10f, 10f ) );
        }

        List<Entity> toDelete = new();
        foreach( var kv in MarkedCrates )
        {
            if ( kv.Key.IsValid() ) continue;
            toDelete.Add( kv.Key );
        }

        foreach( Entity ent in toDelete )
        {
            if( Markers.ContainsKey( MarkedCrates[ent] ) )
            {
                Markers.Remove( MarkedCrates[ent] );
            }

            MarkedCrates[ent].Delete( true );
            MarkedCrates.Remove( ent );
        }

        // Player marker rotation
        PlayerMarker.Style.Dirty();

        PanelTransform tranform = new PanelTransform();
        tranform.AddRotation( 0, 0, 180 + 360 - (currentPawn.EyeRot.Angles().yaw + 180 + 90) );
        tranform.AddTranslateX( Length.Percent( -50f ) );
        tranform.AddTranslateY( Length.Percent( -50f ) );
        PlayerMarker.Style.Transform = tranform;

        // Markers position (should be changed to static on a panel once clipping is available)
        Vector2 playerPos = currentPawn.Position;
        foreach ( var kv in Markers )
        {
            MapMarker markerInfo = kv.Value;
            Panel marker = kv.Key;

            Vector2 pos = markerInfo.WorldPos;

            marker.Style.Dirty();
            marker.Style.Width = markerInfo.Width;
            marker.Style.Height = markerInfo.Height;
            marker.Style.Left = ContentSize / 2 - markerInfo.Width / 2 + ((pos.x - playerPos.x) * PixelsPerUnit);            
            marker.Style.Top = ContentSize / 2 - markerInfo.Height / 2 + -((pos.y - playerPos.y) * PixelsPerUnit);
        }
    }

    private struct MapMarker
    {
        public Vector2 WorldPos;
        public float Width;
        public float Height;

        public MapMarker( Vector3 worldPos, float width, float height )
        {
            WorldPos = worldPos;
            Width = width;
            Height = height;
        }
    }
}
