﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using BattleRoyale;

partial class BRPlayer
{
	[Net]
	public float Armour { get; set; }
	
	[Net]
	public float MaxArmour { get; set; }

	public float FullRegenTime = 6.0f;
	public float RegenStartDelay = 3.0f;

	[Net, Local]
	public bool RegenActive { get; set; } = false;

	[Net, Local]
	public float RegenStartTime { get; set; }

	[Net, Local]
	public float RegenTime { get; set; }

    private TimeSince LastZoneDamage = 0;

    private void ResetRegen()
	{
		if( RegenActive )
		{
			if(Time.Now >= RegenStartTime+RegenStartDelay)
			{
				float regenProgress = (Time.Now - RegenStartTime - RegenStartDelay) / RegenTime;
				Health = Math.Clamp( Health + ((100f - Health) * regenProgress), 0, 100f );
			}

			RegenActive = false;
		}

		if ( Health <= 0 || Health >= 100f ) { return; }

		RegenTime = (1-(Health / 100f))*FullRegenTime;
		RegenStartTime = Time.Now;
		RegenActive = true;
	}

    public bool InDeathZone()
    {
        BRGame game = Game.Current as BRGame;

        Vector2 pos1 = Position;
        Vector2 pos2 = game.ZoneCenterPos;

        float a = pos1.X - pos2.X;
        float b = pos1.Y - pos2.Y;
        double distance = Math.Sqrt( (a * a) + (b * b) );

        return distance >= game.DeathZoneDistance();
    }

	[Event.Tick]
	private void VitalsUpdate()
	{
		if( RegenActive )
        {
            if ( Health >= 100f )
            {
                RegenActive = false;
            }

            if ( Time.Now >= RegenStartTime + RegenStartDelay + RegenTime )
            {
                Health = 100f;
                RegenActive = false;
            }
        }

        if( BRGame.CurrentState == GameState.Active && LastZoneDamage >= 2f )
        {
            if( InDeathZone() ) TakeDamage( DamageInfo.Generic( 5f ) );
            LastZoneDamage = 0;
        }
	}
}
