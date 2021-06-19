using Sandbox;
using BattleRoyale;
using System;

public partial class BRGame
{
    public Vector2 ZoneCenterPos = new Vector2( -5163, 2785 );
    private int ZoneTicks = 0;
    private int ZoneTicksPerUnit = 1;
    private ModelEntity ZoneModel;

    public float DeathZoneDistance()
    {
        return CurrentState == GameState.Active ? Math.Clamp( 7500f - (ZoneTicks / ZoneTicksPerUnit), 0f, 7500f ) : 7500f;
    }

    [Event.Tick]
    private void UpdateZoneMove()
    {
        if( IsClient )
        {
            if( ZoneModel == null || !ZoneModel.IsValid() )
            {
                ZoneModel = new ModelEntity();
                ZoneModel.SetModel( "models/bots/death_zone/cylinder.vmdl_c" );
            }

            ZoneModel.Position = new Vector3( ZoneCenterPos.X, ZoneCenterPos.Y, 300 );
            ZoneModel.Scale = (DeathZoneDistance() * 2) / 100f;
            ZoneModel.RenderColor = Color32.Red;
            ZoneModel.RenderAlpha = .5f;
        }

        if ( CurrentState != GameState.Active ) return;

        ZoneTicks++;
    }
}
