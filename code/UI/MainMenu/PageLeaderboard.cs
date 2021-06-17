
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

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

            CreateLeaderboard( "Most Wins", stats => $"{stats.Kills} Wins", stats => stats.Kills );
            CreateLeaderboard( "Most Kills", stats => $"{stats.Wins} Kills", stats => stats.Wins );
            CreateLeaderboard( "Longest Survived", stats => FormatTime( stats.Survived ), stats => stats.Survived );

            FinishCreating();

            ConsoleSystem.Run( "br_request_data" );
        }

        private void CreateLeaderboard( string title, FormatStat formatStat, GetSortValue getSortValue )
        {
            LeaderboardPanel board = LeaderboardsBack.AddChild<LeaderboardPanel>( "leaderboard" );
            board.SetInfo( title, formatStat, getSortValue );
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

            return $"{hours}h {minutes}m {string.Format( "{0:00}", seconds )}s";
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
    public delegate int GetSortValue( PlayerData.Stats stats );

    public class LeaderboardPanel : Panel
    {
        private Label Title;
        public Button SwitchButton;
        private Panel PlayerList;
        public FormatStat FormatStat;
        public GetSortValue GetSortValue;
        public int SelectionPos;

        public LeaderboardPanel()
        {
            Title = Add.Label( "", "title" );
            SwitchButton = Add.Button( "", "switchbutton" );
            PlayerList = Add.Panel( "playerlist" );
        }
        
        public void SetInfo( string title, FormatStat formatStat, GetSortValue getSortValue )
        {
            Title.Text = title.ToUpper();
            FormatStat = formatStat;
            GetSortValue = getSortValue;
        }

        public void UpdateStats( Dictionary<ulong, PlayerData.Stats> data )
        {
            PlayerList.DeleteChildren();

            List<SortStats> sortedStats = new();
            foreach ( var kv in data )
            {
                sortedStats.Add( new( kv.Key, kv.Value, GetSortValue( kv.Value ) ) );
            }

            sortedStats = sortedStats.OrderByDescending( o => o.SortValue ).ToList();

            for ( int i = 0; i < 10; i++ )
            {
                int currentCount = i + 1;

                Panel entry = PlayerList.Add.Panel( "entry" );
                entry.Add.Label( currentCount <= 3 ? "emoji_events" : $"{currentCount}", "rank" ).AddClass( currentCount <= 3 ? $"rank{currentCount}" : "noicon" );
                entry.SetClass( "last", currentCount >= 10 );

                if( i < sortedStats.Count && sortedStats[i] is SortStats sortStats && sortStats.Stats is PlayerData.Stats stats )
                {
                    entry.Add.Image( $"avatar:{sortStats.SteamID}", "avatar" );
                    entry.Add.Label( stats.Name, "name" );
                    entry.Add.Label( FormatStat( stats ), "statistic" );
                }
            }
        }

        private struct SortStats
        {
            public ulong SteamID;
            public PlayerData.Stats Stats;
            public int SortValue;

            public SortStats( ulong steamID, PlayerData.Stats stats, int sortValue )
            {
                SteamID = steamID;
                Stats = stats;
                SortValue = sortValue;
            }
        }
    }
}
