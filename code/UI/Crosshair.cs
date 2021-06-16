
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class Crosshair : Panel
{
	private int FireCounter;
    private string ActiveClass;

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

		SetClass( "fire", FireCounter > 0 );

		if ( FireCounter > 0 ) FireCounter--;

        if ( Local.Pawn is BRPlayer player && player.ActiveChild is BaseBRWeapon weapon )
        {
            if( ActiveClass != weapon.ClassInfo.Name )
            {
                RemoveClass( ActiveClass );

                ActiveClass = weapon.ClassInfo.Name;
                AddClass( ActiveClass );
            }
        }
    }

    [Event( "battleroyale.weaponfired" )]
    public void OnWeaponFire()
	{
        FireCounter += 2;
    }
}
