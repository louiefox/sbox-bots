using System;
using System.Collections.Generic;
using Sandbox;

namespace BattleRoyale
{
    public partial class PlayerInfo
    {
        public static Dictionary<ulong, PlayerInfo> Players = new();

        public Client Client;
        public PlayerGameState State;
        public int Kills = 0;
        public TimeSince Survived = 0;

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

            foreach( var kv in Players )
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
            if ( GetClientFromSteamID( steamID ) is not Client client ) return;
            PlayerInfo playerInfo = new( client );

            Players.Add( steamID, playerInfo );
            Event.Run( "battleroyale.addplayer", steamID );
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
