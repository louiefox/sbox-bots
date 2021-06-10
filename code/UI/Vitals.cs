
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Vitals : Panel
{
    public Panel ArmourRow;
    public List<Panel> ArmourBars = new();
    public Panel Health;

	public Vitals()
	{
        StyleSheet.Load( "/ui/Vitals.scss" );

        ArmourRow = Add.Panel( "armourrow" );

        Panel healthBar = Add.Panel( "healthbar" );
        Health = healthBar.Add.Panel( "health" );   
	}

    private void UpdateArmourBars( int barCount )
    {
        foreach ( Panel panel in ArmourBars )
        {
            panel?.Delete();
        }

        ArmourBars = new();

        for ( int i = 0; i < barCount; i++ )
        {
            Panel armourBar = ArmourRow.Add.Panel( "armourbar" );

            if ( i == barCount - 1 )
            {
                armourBar.Style.MarginRight = 0;
                armourBar.Style.BorderTopRightRadius = 12;
            }
            else if ( i == 0 )
            {
                armourBar.Style.BorderTopLeftRadius = 12;
            }

            ArmourBars.Add( armourBar.Add.Panel( "armour" ) );
        }
    }

    public override void Tick()
	{
		BRPlayer player = Local.Pawn as BRPlayer;
		if ( player == null ) return;

        int barCount = (int)Math.Ceiling( player.MaxArmour / 50f );
        if ( ArmourBars.Count != barCount )
        {
            UpdateArmourBars( barCount );
        }

        int currentCount = 0;
        foreach ( Panel panel in ArmourBars )
        {
            panel.Style.Dirty();
            panel.Style.Width = Length.Percent( Math.Clamp( (player.Armour - (currentCount * 50f)) / 50f * 100f, 0f, 100f ) );
            panel.Style.BorderTopLeftRadius = currentCount == 0 ? 12 : 0;
            panel.Style.BorderTopRightRadius = currentCount == (ArmourBars.Count - 1) ? 12 : 0;

            currentCount++;
        }

        float extraHealth = 0f;
		if( player.RegenActive && Time.Now >= player.RegenStartTime+player.RegenStartDelay )
		{
			float regenProgress = (Time.Now - player.RegenStartTime - player.RegenStartDelay) / player.RegenTime;
			extraHealth = ((100f - player.Health) * regenProgress);
		}

		Health.Style.Dirty();
		Health.Style.Width = Length.Percent( MathX.Clamp( ((player.Health + extraHealth) / 100f) * 100f, 0f, 100f ) );
	}
}
