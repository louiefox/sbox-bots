
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
        private bool StatPanelsWidthSet = false;
        private List<Dictionary<string, Panel>> StatPanels = new();

        public PageStats()
        {
            StyleSheet.Load( "/ui/MainMenu/PageStats.scss" );

            PlayerList = Add.Panel( "playerlist" );

            UpdateStats();

            ConsoleSystem.Run( "br_request_data" );
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

        private void UpdateStatWidths()
        {
            Dictionary<string, float> maxStatWidths = new();

            foreach ( Dictionary<string, Panel> panels in StatPanels )
            {
                foreach ( var kv in panels )
                {
                    maxStatWidths[kv.Key] = Math.Max( maxStatWidths.ContainsKey( kv.Key ) ? maxStatWidths[kv.Key] : 0f, kv.Value.Style.Width.Value.Value );
                }
            }      
            
            foreach ( Dictionary<string, Panel> panels in StatPanels )
            {
                foreach ( var kv in panels )
                {
                    kv.Value.Style.Dirty();
                    kv.Value.Style.Width = maxStatWidths[kv.Key];
                }
            }

            StatPanelsWidthSet = true;
        }

        public override void Tick()
        {
            base.Tick();

            if( !StatPanelsWidthSet )
            {
                foreach ( Dictionary<string, Panel> panels in StatPanels )
                {
                    bool stop = false;
                    foreach ( var kv in panels )
                    {
                        if ( kv.Value.Style.Width != null )
                        {
                            UpdateStatWidths();
                        }

                        stop = true;
                        break;
                    }

                    if ( stop ) break;
                }
            }
        }

        [Event( "battleroyale.updateplayerdata" )]
        private void UpdateStats()
        {
            PlayerList.DeleteChildren();

            if ( PlayerData.Data == null ) return;

            StatPanels = new();
            StatPanelsWidthSet = false;

            foreach ( var kv in PlayerData.Data )
            {
                PlayerData.Stats stats = kv.Value;

                Panel entry = PlayerList.Add.Panel( "playerentry" );
                entry.Add.Image( $"avatar:{kv.Key}", "playeravatar" );

                Panel playerInfo = entry.Add.Panel( "playerinfo" );
                playerInfo.Add.Label( "Brickwall", "playername" );
                playerInfo.Add.Label( kv.Key.ToString(), "playerid" );

                Dictionary<string, Panel> newStatPanels = new();

                Panel playerStats = entry.AddChild<Panel>( "playerstats" );
                newStatPanels.Add( "survived", playerStats.Add.Label( FormatTime( stats.Survived ), "playerstat" ) );
                newStatPanels.Add( "wins", playerStats.Add.Label( $"{stats.Wins} Wins", "playerstat" ) );
                newStatPanels.Add( "kills", playerStats.Add.Label( $"{stats.Wins} Kills", "playerstat" ) );

                StatPanels.Add( newStatPanels );
            }
        }
    }
}
