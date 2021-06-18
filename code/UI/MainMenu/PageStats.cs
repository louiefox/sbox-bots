
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace BattleRoyale.UI.MainMenuPages
{
    public class PageStats : Panel
    {
        private Panel PlayerList;

        public PageStats()
        {
            StyleSheet.Load( "/ui/MainMenu/PageStats.scss" );

            Panel playerListArea = Add.Panel( "playerlistarea" );

            Panel playerStats = playerListArea.Add.Panel( "headerbar" );
            playerStats.Add.Label( "Player", "header" ).AddClass( "player" );
            playerStats.Add.Label( "Kills", "header" );
            playerStats.Add.Label( "Wins", "header" );
            playerStats.Add.Label( "Survived", "header" ).AddClass( "survived" );

            PlayerList = playerListArea.Add.Panel( "playerlist" );

            UpdateStats();
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
            PlayerList.DeleteChildren();

            if ( PlayerData.Data == null ) return;

            foreach ( var kv in PlayerData.Data )
            {
                PlayerData.Stats stats = kv.Value;

                Panel entry = PlayerList.Add.Panel( "playerentry" );

                Panel playerInfo = entry.Add.Panel( "playerinfo" );
                playerInfo.Add.Image( $"avatar:{kv.Key}", "playeravatar" );

                Panel playerText = playerInfo.Add.Panel( "playertext" );
                playerText.Add.Label( stats.Name, "playername" );
                playerText.Add.Label( kv.Key.ToString(), "playerid" );

                entry.Add.Label( $"{stats.Wins} Kills", "playerstattxt" );
                entry.Add.Label( $"{stats.Wins} Wins", "playerstattxt" );
                entry.Add.Label( FormatTime( stats.Survived ), "playerstattxt" ).AddClass( "survived" );
            }
        }
    }
}
