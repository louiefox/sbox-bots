
using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class GameStatus : Panel
{
	public Panel Waiting;
	public Label WaitingPlayers;	
    
    public Panel Starting;
	public Label StartingTime;    
    
    public Panel Ending;
	public Label EndingTime;

	public GameStatus()
	{
        StyleSheet.Load( "/ui/GameStatus.scss" );

        Waiting = Add.Panel( "waiting" );
        WaitingPlayers = Waiting.Add.Label( "", "waitingplayers" );
        Waiting.Add.Label( "WAITING", "waitingtitle" );

        Starting = Add.Panel( "starting" );
        StartingTime = Starting.Add.Label( "0s", "startingtime" );
        Starting.Add.Label( "STARTING", "startingtitle" );

        Ending = Add.Panel( "starting" );
        EndingTime = Ending.Add.Label( "0s", "startingtime" );
        Ending.Add.Label( "ENDED", "startingtitle" );
    }

    public override void Tick()
	{
        Waiting.SetClass( "active", BRGame.CurrentState == GameState.Waiting );
        Starting.SetClass( "active", BRGame.CurrentState == GameState.Starting );
        Ending.SetClass( "active", BRGame.CurrentState == GameState.Ended );

        if( BRGame.CurrentState == GameState.Waiting )
        {
            WaitingPlayers.Text = $"{Client.All.Count}/10 Players";
        }
        else if ( BRGame.CurrentState == GameState.Starting )
        {
            double time = Math.Round( Math.Max( 0, BRGame.StartDuration - BRGame.StartingTime ), 1 );
            StartingTime.Text = $"{String.Format( "{0:0.0}", time )}s";
        }        
        else if ( BRGame.CurrentState == GameState.Ended )
        {
            double time = Math.Round( Math.Max( 0, BRGame.EndDuration - BRGame.EndedTime ), 1 );
            EndingTime.Text = $"{String.Format( "{0:0.0}", time )}s";
        }
    }
}
