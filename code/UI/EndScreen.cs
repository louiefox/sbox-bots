
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class EndScreen : Panel
{
    private Panel TimeBar;
    private Label TimeText;

    private TimeSince TestTime;

    public EndScreen()
	{
        StyleSheet.Load( "/ui/EndScreen.scss" );

        Panel topPanel = Add.Panel( "toppanel" );
        topPanel.Add.Label( "GAME ENDED" );
        TimeText = topPanel.Add.Label( "", "timetext" );

        Panel timeBar = topPanel.Add.Panel( "timebarback" );
        TimeBar = timeBar.Add.Panel( "timebar" );

        TestTime = 0;
    }

    public override void Tick()
	{
        SetClass( "active", BRGame.CurrentState == GameState.Ended );

        if ( BRGame.CurrentState == GameState.Ended )
        {
            float time = Math.Max( 0, BRGame.EndDuration - BRGame.EndedTime );

            TimeText.Text = $"Next game in {string.Format( "{0:0.0}", time )}s";

            TimeBar.Style.Dirty();
            TimeBar.Style.Width = Length.Percent( time / BRGame.EndDuration * 100f );
        }
    }
}
