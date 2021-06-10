
using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using BattleRoyale;

public class SpectatingInfo : Panel
{
	public Label CurrentTarget;

	public SpectatingInfo()
	{
        StyleSheet.Load( "/ui/SpectatingInfo.scss" );

        Panel CurrentPanel = Add.Panel( "current" );
        CurrentTarget = CurrentPanel.Add.Label( "None", "currenttarget" );
        CurrentPanel.Add.Label( "SPECTATING", "currenttitle" );
    }

    public override void Tick()
	{
        bool IsSpectating = BRGame.IsSpectating();
        SetClass( "active", IsSpectating );

        if ( !IsSpectating || Local.Client.Camera is not BRSpectateCamera camera || camera.CurrentTarget is not Player currentTarget ) return;

        CurrentTarget.Text = currentTarget.GetClientOwner().Name;
    }
}
