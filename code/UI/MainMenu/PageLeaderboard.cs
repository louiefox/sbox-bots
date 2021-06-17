
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace BattleRoyale.UI.MainMenuPages
{
    public class PageLeaderboard : Panel
    {
        private Panel LeaderboardsBack;
        private List<LeaderboardPanel> Leaderboards = new();
        private LeaderboardPanel TargetLeaderboard;
        private float TargetLeft = 0f;

        public PageLeaderboard()
        {
            StyleSheet.Load( "/ui/MainMenu/PageLeaderboard.scss" );

            LeaderboardsBack = Add.Panel( "leaderboardscontainer" );

            CreateLeaderboard( "Most Wins", stats => $"{stats.Kills} Wins" );
            CreateLeaderboard( "Most Kills", stats => $"{stats.Wins} Kills" );
            CreateLeaderboard( "Longest Survived", stats => FormatTime( stats.Survived ) );

            FinishCreating();

            ConsoleSystem.Run( "br_request_data" );
        }

        private void CreateLeaderboard( string title, FormatStat formatStat )
        {
            LeaderboardPanel board = LeaderboardsBack.AddChild<LeaderboardPanel>( "leaderboard" );
            board.SetInfo( title, formatStat );
            board.SwitchButton.AddEvent( "onclick", () => {
                SetTargetLeaderboard( board );
            } );
            board.SwitchButton.AddClass( "inactive" );
            board.SelectionPos = Leaderboards.Count + 1;

            Leaderboards.Add( board );
        }

        private void FinishCreating()
        {
            int targetIndex = (int)Math.Ceiling( Leaderboards.Count / 2f ) - 1;
            SetTargetLeaderboard( Leaderboards[targetIndex] );

            UpdateStats();
        }

        private void SetTargetLeaderboard( LeaderboardPanel newTarget )
        {
            if ( TargetLeaderboard == newTarget ) return;

            if( TargetLeaderboard != null )
            {
                TargetLeaderboard.RemoveClass( "target" );
                TargetLeaderboard.SwitchButton.AddClass( "inactive" );

                LeaderboardsBack.SetClass( "moveleft", newTarget.SelectionPos == 1 );
                LeaderboardsBack.SetClass( "moveright", newTarget.SelectionPos == 3 );
            }

            TargetLeaderboard = newTarget;
            newTarget.AddClass( "target" );
            newTarget.SwitchButton.RemoveClass( "inactive" );
        }

        public override void Tick()
        {
            base.Tick();
        }

        private string FormatTime( double secs )
        {
            double seconds = secs;

            double hours = Math.Floor( seconds / 60f / 60f );
            seconds -= hours * 60f * 60f;

            double minutes = Math.Floor( seconds / 60f );
            seconds -= minutes * 60f;

            return $"{hours}h {minutes}m {seconds}s";
        }

        [Event( "battleroyale.updateplayerdata" )]
        private void UpdateStats()
        {
            if ( PlayerData.Data == null ) return;

            foreach( LeaderboardPanel panel in Leaderboards )
            {
                panel.UpdateStats( PlayerData.Data );
            }
        }
    }

    public delegate string FormatStat( PlayerData.Stats stats );

    public class LeaderboardPanel : Panel
    {
        private Label Title;
        public Button SwitchButton;
        private Panel PlayerList;
        public FormatStat FormatStat;
        public int SelectionPos;

        public LeaderboardPanel()
        {
            Title = Add.Label( "", "title" );
            SwitchButton = Add.Button( "", "switchbutton" );
            PlayerList = Add.Panel( "playerlist" );
        }
        
        public void SetInfo( string title, FormatStat formatStat )
        {
            Title.Text = title.ToUpper();
            FormatStat = formatStat;
        }

        public void UpdateStats( Dictionary<ulong, PlayerData.Stats> data )
        {
            PlayerList.DeleteChildren();

            int currentCount = 0;
            foreach ( var kv in data )
            {
                currentCount++;
                if ( currentCount > 10 ) break;

                PlayerData.Stats stats = kv.Value;

                Panel entry = PlayerList.Add.Panel( "entry" );
                entry.Add.Label( currentCount <= 3 ? "emoji_events" : $"{currentCount}", "rank" ).AddClass( currentCount <= 3 ? $"rank{currentCount}" : "noicon" );
                entry.Add.Image( $"avatar:{kv.Key}", "avatar" );
                entry.Add.Label( stats.Name, "name" );
                entry.Add.Label( FormatStat( stats ), "statistic" );
                entry.SetClass( "last", currentCount >= data.Count );
            }
        }
    }
}
