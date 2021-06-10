
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using BattleRoyale;

public class SpectatingInfo : Panel
{
	public Panel ArmourRow;
	public List<Panel> ArmourBars = new();
    public Panel Health;
    public Label CurrentTarget;

    public SpectatingInfo()
	{
        StyleSheet.Load( "/ui/SpectatingInfo.scss" );

        Panel vitalsPanel = Add.Panel( "vitals" );
        ArmourRow = vitalsPanel.Add.Panel( "armourrow" );
        Panel healthBar = vitalsPanel.Add.Panel( "healthbar" );
        Health = healthBar.Add.Panel( "health" );        

        Panel currentPanel = Add.Panel( "current" );
        CurrentTarget = currentPanel.Add.Label( "None", "currenttarget" );
        currentPanel.Add.Label( "SPECTATING", "currenttitle" );
    }

    private void UpdateArmourBars( int barCount )
    {
        foreach( Panel panel in ArmourBars )
        {
            panel?.Delete();
        }

        ArmourBars = new();

        for( int i = 0; i < barCount; i++ )
        {
            Panel armourBar = ArmourRow.Add.Panel( "armourbar" );
            
            if( i == barCount-1 )
            {
                armourBar.Style.MarginRight = 0;
                armourBar.Style.BorderTopRightRadius = 12;
            } else if( i == 0 )
            {
                armourBar.Style.BorderTopLeftRadius = 12;
            }

            ArmourBars.Add( armourBar.Add.Panel( "armour" ) );
        }
    }

    public override void Tick()
	{
        bool IsSpectating = BRGame.IsSpectating();
        SetClass( "active", IsSpectating );

        if ( !IsSpectating || Local.Client.Camera is not BRSpectateCamera camera || camera.CurrentTarget is not BRPlayer currentTarget ) return;

        Health.Style.Dirty();
        Health.Style.Width = Length.Percent( Math.Clamp( (currentTarget.Health / 100f) * 100f, 0f, 100f ) );

        int barCount = (int)Math.Ceiling( currentTarget.MaxArmour / 50f );
        if ( ArmourBars.Count != barCount )
        {
            UpdateArmourBars( barCount );
        }

        int currentCount = 0;
        foreach( Panel panel in ArmourBars )
        {
            panel.Style.Dirty();
            panel.Style.Width = Length.Percent( Math.Clamp( (currentTarget.Armour - (currentCount * 50f)) / 50f * 100f, 0f, 100f ) );
            panel.Style.BorderTopLeftRadius = currentCount == 0 ? 12 : 0;
            panel.Style.BorderTopRightRadius = currentCount == (ArmourBars.Count - 1) ? 12 : 0;

            currentCount++;
        }

        CurrentTarget.Text = currentTarget.GetClientOwner().Name;
    }
}
