
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class ZoneWarnings : Panel
{
	private TimeSince LastJitter;
	private bool JitterLeft;

	public ZoneWarnings()
	{
        StyleSheet.Load( "/ui/ZoneWarnings.scss" );

        Add.Label( "warning", "warningicon" );

        Panel textContainer = Add.Panel( "textcontainer" );
		textContainer.Add.Label( "WARNING DEATH ZONE", "deathzone" );
		textContainer.Add.Label( "Exit immediately to avoid damage!", "hint" );
    }

    public override void Tick()
	{
        if ( Local.Pawn is not BRPlayer player ) return;

		bool active = player.InDeathZone();
		SetClass( "active", active );

		if( active && LastJitter >= .05f )
		{
			LastJitter = 0;
			JitterLeft = !JitterLeft;

			SetClass( "jitterleft", JitterLeft );
			SetClass( "jitterright", !JitterLeft );
		}
    }
}
