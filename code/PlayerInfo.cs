using System;
using System.Collections.Generic;
using Sandbox;

namespace BattleRoyale
{
    public partial class PlayerInfo
    {
        public static Dictionary<ulong, PlayerInfo> Players = new();
        public static Dictionary<ulong, (TimeSince, int)> DelayedSendAddPlayer = new();

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
            if ( Players.ContainsKey( client.SteamId ) ) return;

            Players.Add( client.SteamId, new( client ) );

            SendAddPlayer( To.Everyone, client.SteamId );

            foreach ( var kv in Players )
            {
                if ( kv.Key == client.SteamId ) continue;
                SendAddPlayer( To.Single( client ), kv.Key );
            }
        }
        
        public static void RemovePlayer( Client client )
        {
            if ( !Players.ContainsKey( client.SteamId ) ) return;

            Players.Remove( client.SteamId );
            SendRemovePlayer( To.Everyone, client.SteamId );
        }
        
        public static PlayerInfo GetPlayerInfo( ulong steamID )
        {
            if ( !Players.ContainsKey( steamID ) ) return null;
            return Players[steamID];
        }

        public static PlayerInfo GetPlayerInfo( Player player )
        {
            return GetPlayerInfo( player.GetClientOwner().SteamId );
        }        
        
        public static PlayerInfo GetPlayerInfo( Client client )
        {
            return GetPlayerInfo( client.SteamId );
        }    
        
        public static void UpdateGameState( Client client, PlayerGameState state )
        {
            if ( GetPlayerInfo( client ) is not PlayerInfo playerInfo ) return;
            playerInfo.State = state;

            if ( state == PlayerGameState.Alive )
            {
                playerInfo.AliveSince = 0;
            }
            else if ( state == PlayerGameState.Dead )
            {
                playerInfo.survived = playerInfo.AliveSince;
            }

            UpdateGameState( To.Everyone, playerInfo.Client.SteamId, state );
        }

        public static void UpdateGameState( Player player, PlayerGameState state )
        {
            UpdateGameState( player.GetClientOwner(), state );
        }

        [ClientRpc]
        public static void UpdateGameState( ulong steamID, PlayerGameState state )
        {
            if ( GetPlayerInfo( steamID ) is not PlayerInfo playerInfo ) return;
            playerInfo.State = state;

            Event.Run( "battleroyale.updateplayer", steamID );
        }

        public static void UpdateKills( Player player, int kills, bool setKills )
        {
            if ( GetPlayerInfo( player ) is not PlayerInfo playerInfo ) return;
            playerInfo.Kills = setKills ? kills : playerInfo.Kills + kills;

            UpdateKills( To.Everyone, playerInfo.Client.SteamId, playerInfo.Kills );
        }        
        
        public static void UpdateKills( Player player, int kills )
        {
            UpdateKills( player, kills, false );
        }

        [ClientRpc]
        public static void UpdateKills( ulong steamID, int kills )
        {
            if ( GetPlayerInfo( steamID ) is not PlayerInfo playerInfo ) return;
            playerInfo.Kills = kills;

            Event.Run( "battleroyale.updateplayer", steamID );
        }

        public static Client GetClientFromSteamID( ulong steamID )
        {
            foreach( Client client in Client.All )
            {
                if ( client.SteamId == steamID ) return client;
            }

            return null;
        }

        [ClientRpc]
        public static void SendAddPlayer( ulong steamID )
        {
            DelayedSendAddPlayer.Add( steamID, (0, 0) );
        }

        [Event( "client.tick" )]
        public static void DelayedSendPlayerUpdate()
        {
            List<ulong> toDelete = new();
            foreach ( var kv in DelayedSendAddPlayer )
            {
                if ( kv.Value.Item1 < .5f ) continue;

                DelayedSendAddPlayer[kv.Key] = (0, kv.Value.Item2 + 1);

                if ( GetClientFromSteamID( kv.Key ) is Client client )
                {
                    PlayerInfo playerInfo = new( client );

                    Players.Add( kv.Key, playerInfo );
                    Event.Run( "battleroyale.addplayer", kv.Key );
                    toDelete.Add( kv.Key );
                    continue;
                }

                if ( kv.Value.Item2 >= 5 ) toDelete.Add( kv.Key );
            }

            foreach ( ulong steamID in toDelete )
            {
                DelayedSendAddPlayer.Remove( steamID );
            }
        }

        [ClientRpc]
        public static void SendRemovePlayer( ulong steamID )
        {
            if ( !Players.ContainsKey( steamID ) ) return;

            Players.Remove( steamID );
            Event.Run( "battleroyale.removeplayer", steamID );
        }
    }

    public enum PlayerGameState
    {
        Alive,
        Dead,
        Spectating
    }
}
