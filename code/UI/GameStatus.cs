
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class GameStatus : Panel
{
	public Panel Waiting;
	public Label WaitingPlayers;	
    
    public Panel Starting;
	public Label StartingTime;
    public List<Panel> StartingTimeBars = new();

	public GameStatus()
	{
        StyleSheet.Load( "/ui/GameStatus.scss" );

        // Waiting
        Waiting = Add.Panel( "waiting" );
        WaitingPlayers = Waiting.Add.Label( "", "waitingplayers" );
        Waiting.Add.Label( "WAITING", "waitingtitle" );

        // Starting
        Starting = Add.Panel( "starting" );

        Panel timerColumn = Starting.Add.Panel( "timercolumn" );
        int barCount = 5;
        for ( int i = 0; i < barCount; i++ )
        {
            Panel timerBar = timerColumn.Add.Panel( "timerbarslot" );
            timerBar.Style.MarginBottom = i < barCount-1 ? 3 : 0;

            StartingTimeBars.Add( timerBar.Add.Panel( "timerbar" ) );
        }

        Panel startingText = Starting.Add.Panel( "startingtext" );
        StartingTime = startingText.Add.Label( "0s", "startingtime" );
        startingText.Add.Label( "GAME STARTING", "startingtitle" );
    }

    public override void Tick()
	{
        GameState currentState = BRGame.CurrentState;

        Waiting.SetClass( "active", currentState == GameState.Waiting );
        Starting.SetClass( "active", currentState == GameState.Starting );

        if( currentState == GameState.Waiting )
        {
            WaitingPlayers.Text = $"{Client.All.Count}/10 Players";
        }
        else if ( currentState == GameState.Starting )
        {
            double time = Math.Max( 0, BRGame.StartDuration - BRGame.StartingTime );
            StartingTime.Text = $"{string.Format( "{0:0.0}", time )}s";

            float slotDuration = BRGame.StartDuration / StartingTimeBars.Count;
            int currentCount = 0;
            foreach( Panel panel in StartingTimeBars )
            {
                currentCount++;
                int count = currentCount;

                float percent = ((float)time - (BRGame.StartDuration - slotDuration * count)) / 1f;
                if ( (panel.Style.Opacity <= 0f && percent <= 0f) || (panel.Style.Opacity >= 1f && percent >= 1f) ) continue;

                panel.Style.Dirty();
                panel.Style.Opacity = Math.Clamp( percent, 0f, 1f );
            }
        }
    }
}
