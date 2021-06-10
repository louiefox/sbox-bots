﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

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

	[Event.Tick]
	private void Regen()
	{
		if( !RegenActive ) { return; }

		if( Health >= 100f )
		{
			RegenActive = false;
		}

		if( Time.Now >= RegenStartTime+RegenStartDelay+RegenTime )
		{
			Health = 100f;
			RegenActive = false;
		}
	}
}
