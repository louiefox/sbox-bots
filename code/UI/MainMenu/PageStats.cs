
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
        private VirtualScrollPanel<PageStatsEntry> PlayerList;

        public PageStats()
        {
            StyleSheet.Load( "/ui/MainMenu/PageStats.scss" );

            Panel playerListArea = Add.Panel( "playerlistarea" );

			TextEntry search = playerListArea.Add.TextEntry( "" );
			search.Placeholder = "Search";
			search.AddEvent( "onchange", () => UpdateStats( search.Text ) );

			Panel playerStats = playerListArea.Add.Panel( "headerbar" );
            playerStats.Add.Label( "Player", "header" ).AddClass( "player" );
            playerStats.Add.Label( "Kills", "header" );
            playerStats.Add.Label( "Wins", "header" );
            playerStats.Add.Label( "Survived", "header" ).AddClass( "survived" );

            PlayerList = playerListArea.AddChild<VirtualScrollPanel<PageStatsEntry>>( "playerlist");
			PlayerList.Layout.ItemSize = new Vector2( 0, 50 );
			PlayerList.Layout.AutoColumns = true;

			UpdateStats();
        }

        [Event( "battleroyale.updateplayerdata" )]
        private void UpdateStats(string filter = "")
        {
            PlayerList.Clear();

            if ( PlayerData.Data == null ) return;

			filter = filter.ToLower();

			PlayerList.Data.AddRange( PlayerData.Data.Select( x => (object)x ).Where( x => 
			{
				if ( x == null || x is not KeyValuePair<ulong, PlayerData.Stats> kv ) return false;


				return kv.Value.Name.ToLower().Contains( filter );
			} ) );
        }
    }

	public class PageStatsEntry : Panel
	{
		public PageStatsEntry()
		{
			SetClass( "playerentry", true );
		}

		public override void SetDataObject( object obj )
		{
			base.SetDataObject( obj );

			if ( obj is not KeyValuePair<ulong, PlayerData.Stats> kv ) return;

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
