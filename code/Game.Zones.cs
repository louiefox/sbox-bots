using Sandbox;
using BattleRoyale;
using System;

public partial class BRGame
{
    public Vector2 ZoneCenterPos = new Vector2( -3491, -3572 );
	private float ZoneMaxDistance = 8000f;
    private int ZoneTicks = 0;
    private int ZoneTicksPerUnit = 1;
    private ModelEntity ZoneModel;

    public float DeathZoneDistance()
    {
        return CurrentState == GameState.Active ? Math.Clamp( ZoneMaxDistance - (ZoneTicks / ZoneTicksPerUnit), 0f, ZoneMaxDistance ) : ZoneMaxDistance;
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

            ZoneModel.Position = new Vector3( ZoneCenterPos.x, ZoneCenterPos.y, 300 );
            ZoneModel.Scale = (DeathZoneDistance() * 2) / 100f;
            ZoneModel.RenderColor = Color.Red;
            ZoneModel.RenderAlpha = .5f;
        }

        if ( CurrentState != GameState.Active ) return;

        ZoneTicks++;
    }
}
