
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale.UI.MainMenuPages
{
    public class PageStats : Panel
    {
        private Panel PlayerList;

        public PageStats()
        {
            StyleSheet.Load( "/ui/MainMenu/PageStats.scss" );

            Panel playerListArea = Add.Panel( "playerlistarea" );

			TextEntry search = playerListArea.Add.TextEntry( "" );
			search.Placeholder = "Search";
			search.AddEventListener( "onchange", () => RefreshStats( search.Text ) );

			Panel playerStats = playerListArea.Add.Panel( "headerbar" );
            playerStats.Add.Label( "Player", "header" ).AddClass( "player" );
            playerStats.Add.Label( "Kills", "header" );
            playerStats.Add.Label( "Wins", "header" );
            playerStats.Add.Label( "Survived", "header" ).AddClass( "survived" );

            PlayerList = playerListArea.Add.Panel( "playerlist");

			RefreshStats();
        }

        [Event( "battleroyale.updateplayerdata" )]
        private void RefreshStats()
        {
			RefreshStats( "" );
		}        
		
		private void RefreshStats(string filter)
        {
			PlayerList.DeleteChildren();

            if ( PlayerData.Data == null ) return;

			filter = filter.ToLower();

			foreach( KeyValuePair<ulong, PlayerData.Stats> kv in PlayerData.Data )
			{
				if ( !kv.Value.Name.ToLower().Contains( filter ) ) continue;

				PlayerList.AddChild<PageStatsEntry>( "playerentry" ).SetData( kv );
			}
        }
    }

	public class PageStatsEntry : Panel
	{
		public void SetData( KeyValuePair<ulong, PlayerData.Stats> kv )
		{
			PlayerData.Stats stats = kv.Value;

			Panel playerInfo = Add.Panel( "playerinfo" );
			playerInfo.Add.Image( $"avatar:{kv.Key}", "playeravatar" );

			Panel playerText = playerInfo.Add.Panel( "playertext" );
			playerText.Add.Label( stats.Name, "playername" );
			playerText.Add.Label( kv.Key.ToString(), "playerid" );

			Add.Label( $"{stats.Kills} Kills", "playerstattxt" );
			Add.Label( $"{stats.Wins} Wins", "playerstattxt" );
			Add.Label( FormatTime( stats.Survived ), "playerstattxt" ).AddClass( "survived" );
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
	}
}
