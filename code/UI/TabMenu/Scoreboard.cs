
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace BattleRoyale.UI
{
    public class Scoreboard : Panel
    {
        private Dictionary<ulong, ScoreboardEntry> Players = new();
        private Dictionary<string, Panel> Sections = new();

        public Scoreboard()
        {
            StyleSheet.Load( "/ui/TabMenu/Scoreboard.scss" );

            AddSection( "Alive", "alive" );
            AddSection( "Dead", "Dead" );
            AddSection( "Spectating", "Spectating" );

            foreach ( var kv in PlayerInfo.Players )
            {
                AddPlayer( kv.Key );
            }
        }

        private void AddSection( string title, string id )
        {
            Panel section = Add.Panel( "section" );

            Panel titlePanel = section.Add.Panel( "titlepanel" );
            titlePanel.AddClass( id );
            titlePanel.Add.Label( title, "title" );

            Panel header = section.Add.Panel( "header" );
            header.Add.Label( "Name", "name" );
            header.Add.Label( "Kills", "kills" );
            header.Add.Label( "Survived", "survived" );
            header.Add.Label( "Ping", "ping" );

            Sections.Add( id, section );
        }

        [Event( "battleroyale.addplayer" )]
        public void AddPlayer( ulong steamID )
        {
            if ( Players.ContainsKey( steamID ) ) return;
            PlayerInfo playerInfo = PlayerInfo.Players[steamID];

            ScoreboardEntry panel = Sections["alive"].AddChild<ScoreboardEntry>( "player" );
            panel.SetInfo( playerInfo );

            Players.Add( steamID, panel );
        }

        [Event( "battleroyale.removeplayer" )]
        private void RemovePlayer( ulong steamID )
        {
            if ( !Players.ContainsKey( steamID ) ) return;

            Players[steamID].Delete();
            Players.Remove( steamID );
        }

        [Event( "battleroyale.updateplayer" )]
        private void UpdatePlayer( ulong steamID )
        {
            if ( !Players.ContainsKey( steamID ) ) return;
            Players[steamID]?.UpdateInfo();
        }
    }

    public class ScoreboardEntry : Panel
    {
        public PlayerInfo Info;

        private Label Kills;
        private Label Survived;
        private Label Ping;
        private TimeSince LastUpdate;

        public void SetInfo( PlayerInfo playerInfo )
        {
            Info = playerInfo;

            Add.Label( playerInfo.Client.Name, "name" );

            Kills = Add.Label( playerInfo.Kills.ToString(), "kills" );
            Survived = Add.Label( playerInfo.Survived.ToString(), "survived" );
            Ping = Add.Label( "0", "ping" );
        }

        public void UpdateInfo()
        {
            Kills.Text = Info.Kills.ToString();
        }

        [Event.Tick]
        private void Update()
        {
            if ( LastUpdate < 5 ) return;
            LastUpdate = 0;

            double minutes = Math.Floor( Info.Survived / 60f );
            double seconds = Math.Floor( Info.Survived - (minutes * 60f));

            Survived.Text = $"{minutes}m {seconds}s";
        }
    }
}
