
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace BattleRoyale.UI
{
    public class Scoreboard : Panel
    {
        private Dictionary<Client, ScoreboardEntry> Players = new();
        private Dictionary<string, Panel> Sections = new();

        public Scoreboard()
        {
            StyleSheet.Load( "/ui/TabMenu/Scoreboard.scss" );

            AddSection( "Alive", "alive" );
            AddSection( "Dead", "dead" );
            AddSection( "Spectating", "spectating" );

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
        public void AddPlayer( Client client )
        {
            if ( Players.ContainsKey( client ) ) return;
            PlayerInfo playerInfo = PlayerInfo.Players[client];

            ScoreboardEntry panel = Sections[playerInfo.State.ToString().ToLower()].AddChild<ScoreboardEntry>( "player" );
            panel.SetInfo( playerInfo );

            Players.Add( client, panel );
        }

        [Event( "battleroyale.removeplayer" )]
        private void RemovePlayer( Client client )
        {
            if ( !Players.ContainsKey( client ) ) return;

            Players[client].Delete();
            Players.Remove( client );
        }

        [Event( "battleroyale.updateplayer" )]
        private void UpdatePlayer( Client client )
        {
            if ( !Players.ContainsKey( client ) ) return;
            ScoreboardEntry entry = Players[client];

            entry.Parent = Sections[entry.Info.State.ToString().ToLower()];
            entry.UpdateInfo();
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

        public override void Tick()
        {
            if ( LastUpdate < 5 ) return;
            LastUpdate = 0;

            double minutes = Math.Floor( Info.Survived / 60f );
            double seconds = Math.Floor( Info.Survived - (minutes * 60f));

            Survived.Text = $"{minutes}m {seconds}s";
        }
    }
}
