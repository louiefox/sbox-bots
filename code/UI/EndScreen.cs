
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using BattleRoyale;

public class EndScreen : Panel
{
    private Panel TimeBar;
    private Label TimeText;
    private Label WinnerText;

    public EndScreen()
	{
        StyleSheet.Load( "/ui/EndScreen.scss" );

        Panel topPanel = Add.Panel( "toppanel" );
        topPanel.Add.Label( "GAME ENDED" );
        TimeText = topPanel.Add.Label( "", "timetext" );

        Panel timeBar = topPanel.Add.Panel( "timebarback" );
        TimeBar = timeBar.Add.Panel( "timebar" );

        Panel winnerPanel = topPanel.Add.Panel( "winnerback" );
        winnerPanel.Add.Panel( "winnerbackground" );

        Panel winnerContent = winnerPanel.Add.Panel( "winnercontent" );
        winnerContent.Add.Label( "THE WINNER IS", "winnertitle" );
        WinnerText = winnerContent.Add.Label( "BRICKWALL", "winnertext" );
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

            Client winner = null;
            foreach( var kv in PlayerInfo.Players )
            {
                if ( kv.Value.State != PlayerGameState.Alive ) continue;
                winner = kv.Value.Client;
                break;
            }

            if( winner != null )
            {
                WinnerText.Text = winner.Name.ToUpper();
            }
        }
    }
}
