using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

partial class BRPlayer
{
	[Net, Local]
	public int MaxHealth { get; set; }

	[Net, Local]
	public int Armour { get; set; }	
	
	[Net, Local]
	public int MaxArmour { get; set; }

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
				Health = Math.Clamp( Health + ((MaxHealth - Health) * regenProgress), 0, MaxHealth );
			}

			RegenActive = false;
		}

		if ( Health <= 0 || Health >= MaxHealth ) { return; }

		RegenTime = (1-((float)Health / (float)MaxHealth))*FullRegenTime;
		RegenStartTime = Time.Now;
		RegenActive = true;
	}

	[Event.Tick]
	private void Regen()
	{
		if( !RegenActive ) { return; }

		if( Health >= MaxHealth )
		{
			RegenActive = false;
		}

		if( Time.Now >= RegenStartTime+RegenStartDelay+RegenTime )
		{
			Health = MaxHealth;
			RegenActive = false;
		}
	}
}
