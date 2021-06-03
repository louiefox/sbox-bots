
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class Crosshair : Panel
{
	int fireCounter;

	public Crosshair()
	{
		StyleSheet.Load( "/ui/Crosshair.scss" );

		Style.Left = Length.Fraction( .5f );
		Style.Top = Length.Fraction( .5f );

		for ( int i=0; i<5; i++ )
		{
			var p = Add.Panel( "element" );
			p.AddClass( $"el{i}" );
		}
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "fire", fireCounter > 0 );

		if ( fireCounter > 0 )
			fireCounter--;
	}

	public override void OnEvent( string eventName )
	{
		if ( eventName == "fire" )
		{
			// this is a hack until we have animation or TriggerClass support
			fireCounter += 2;
		}

		base.OnEvent( eventName );
	}
}
