using System;
using System.Collections.Generic;
using Sandbox;

namespace BattleRoyale
{
    public partial class PlayerInfo
    {
        public static Dictionary<Client, PlayerInfo> Players = new();
        public static Dictionary<Client, (TimeSince, int)> DelayedSendAddPlayer = new();

        public Client Client;
        public PlayerGameState State;
        public int Kills = 0;
        public float Survived
        {
            get
            {
                if ( State == PlayerGameState.Alive ) return AliveSince;
                else if ( State == PlayerGameState.Dead ) return survived;

                return 0;
            }
        }

        private float survived;
        private TimeSince AliveSince = 0;

        public PlayerInfo( Client client )
        {
            Client = client;
            State = PlayerGameState.Spectating;
        }

        public static void AddPlayer( Client client )
        {
            if ( Players.ContainsKey( client ) ) return;

            Players.Add( client, new( client ) );

            SendAddPlayer( To.Everyone, client );

            foreach ( var kv in Players )
            {
                if ( kv.Key == client ) continue;
                SendAddPlayer( To.Single( client ), kv.Key );
            }
        }
        
        public static void RemovePlayer( Client client )
        {
            if ( !Players.ContainsKey( client ) ) return;

            Players.Remove( client );
            SendRemovePlayer( To.Everyone, client );
        }
        
        public static PlayerInfo GetPlayerInfo( Client client )
        {
            if ( !Players.ContainsKey( client ) ) return null;
            return Players[client];
        }

        public static PlayerInfo GetPlayerInfo( Player player )
        {
            return GetPlayerInfo( player.GetClientOwner() );
        }         
        
        public static void UpdateGameState( Client client, PlayerGameState state )
        {
            if ( GetPlayerInfo( client ) is not PlayerInfo playerInfo ) return;

            switch ( state )
            {
                case PlayerGameState.Alive:
                    playerInfo.AliveSince = 0;
                    break;
                case PlayerGameState.Dead:
                    playerInfo.survived = playerInfo.AliveSince;
                    break;
            }

            if( state != PlayerGameState.Dead ) playerInfo.Kills = 0;

            playerInfo.State = state;

            SendUpdateGameState( To.Everyone, playerInfo.Client, state );
        }

        public static void UpdateGameState( Player player, PlayerGameState state )
        {
            UpdateGameState( player.GetClientOwner(), state );
        }

        [ClientRpc]
        public static void SendUpdateGameState( Client client, PlayerGameState state )
        {
            if ( GetPlayerInfo( client ) is not PlayerInfo playerInfo ) return;
            playerInfo.State = state;

            switch ( state )
            {
                case PlayerGameState.Alive:
                    playerInfo.AliveSince = 0;
                    break;
                case PlayerGameState.Dead:
                    playerInfo.survived = playerInfo.AliveSince;
                    break;
            }

            if ( state != PlayerGameState.Dead ) playerInfo.Kills = 0;

            Event.Run( "battleroyale.updateplayer", client );
        }

        public static void UpdateKills( Client client, int kills, bool setKills )
        {
            if ( GetPlayerInfo( client ) is not PlayerInfo playerInfo ) return;
            playerInfo.Kills = setKills ? kills : playerInfo.Kills + kills;

            SendUpdateKills( To.Everyone, playerInfo.Client, playerInfo.Kills );
        }           
        
        public static void UpdateKills( Player player, int kills, bool setKills )
        {
            UpdateKills( player.GetClientOwner(), kills, setKills );
        }        
        
        public static void UpdateKills( Player player, int kills )
        {
            UpdateKills( player, kills, false );
        }

        [ClientRpc]
        public static void SendUpdateKills( Client client, int kills )
        {
            if ( GetPlayerInfo( client ) is not PlayerInfo playerInfo ) return;
            playerInfo.Kills = kills;

            Event.Run( "battleroyale.updateplayer", client );
        }

        [ClientRpc]
        public static void SendAddPlayer( Client client )
        {
            DelayedSendAddPlayer.Add( client, (0, 0) );
        }

        [Event( "client.tick" )]
        public static void DelayedSendPlayerUpdate()
        {
            List<Client> toDelete = new();
            foreach ( var kv in DelayedSendAddPlayer )
            {
                if ( kv.Value.Item1 < .5f ) continue;

                DelayedSendAddPlayer[kv.Key] = (0, kv.Value.Item2 + 1);

                if ( kv.Key is Client client )
                {
                    PlayerInfo playerInfo = new( client );

                    Players.Add( client, playerInfo );
                    Event.Run( "battleroyale.addplayer", kv.Key );
                    toDelete.Add( client );
                    continue;
                }

                if ( kv.Value.Item2 >= 120 ) toDelete.Add( kv.Key );
            }

            foreach ( Client client in toDelete )
            {
                DelayedSendAddPlayer.Remove( client );
            }
        }

        [ClientRpc]
        public static void SendRemovePlayer( Client client )
        {
            if ( !Players.ContainsKey( client ) ) return;

            Players.Remove( client );
            Event.Run( "battleroyale.removeplayer", client );
        }
    }

    public enum PlayerGameState
    {
        Alive,
        Dead,
        Spectating
    }
}
