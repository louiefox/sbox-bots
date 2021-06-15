using Sandbox;
using BattleRoyale;
using System;

public partial class BRGame
{
    public Vector2 ZoneCenterPos = new Vector2( -5163, 2785 );
    private int ZoneTicks = 0;
    private int ZoneTicksPerUnit = 10;

    public float DeathZoneDistance()
    {
        return CurrentState == GameState.Active ? Math.Clamp( 7500f - (ZoneTicks / ZoneTicksPerUnit), 0f, 7500f ) : 7500f;
    }

    [Event.Tick]
    private void UpdateZoneMove()
    {
        if ( CurrentState != GameState.Active ) return;

        ZoneTicks++;
    }
}
