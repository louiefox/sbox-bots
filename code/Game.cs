using Sandbox;
using BattleRoyale;
using System;
using System.Collections.Generic;
using System.Linq;

[Library( "bots", Title = "Battle of Terrys" )]
public partial class BRGame : Game
{
    public static GameState CurrentState = GameState.Waiting;
    public static TimeSince StartingTime;
    public static TimeSince EndedTime;
    public static float StartDuration = 30f;
    public static float EndDuration = 15f;

    private BattleRoyaleHUD BattleRoyaleHUD;
    private Dictionary<Client, TimeSince> DelayedClients = new();

    public BRGame()
    {
        if ( IsClient ) BattleRoyaleHUD = new BattleRoyaleHUD();

        if ( IsServer )
        {
            PlayerData.LoadData();
        }
    }

    [Event.Hotload]
    public void UpdateHUD()
    {
        if ( !IsClient || BattleRoyaleHUD == null ) { return; }
        BattleRoyaleHUD.Delete();
        BattleRoyaleHUD = new BattleRoyaleHUD();
    }

    [Event( "server.tick" )]
    public void GameTick()
    {
        return;
        if ( CurrentState == GameState.Waiting )
        {
            if ( Client.All.Count > 1 ) StartStarting();
        }
        else if ( CurrentState == GameState.Starting )
        {
            if ( Client.All.Count <= 1 ) StartWaiting();

            if ( StartingTime >= StartDuration ) StartGame();
        }
        else if ( CurrentState == GameState.Active )
        {
            CheckGameEnd( 0 );
        }        
        else if ( CurrentState == GameState.Ended )
        {
            if ( EndedTime > EndDuration ) StartWaiting();
        }
    }

    public void CheckGameEnd( int takeAmount )
    {
        if ( CurrentState != GameState.Ended ) return;

        int alivePlayers = takeAmount;
        foreach ( var kv in PlayerInfo.Players )
        {
            if ( kv.Value.State != PlayerGameState.Alive ) continue;
            alivePlayers++;
        }

        if ( Client.All.Count <= 1 || alivePlayers <= 1 ) EndGame();
    }

    public void StartWaiting()
    {
        CurrentState = GameState.Waiting;

        SendStartWaiting( To.Everyone );

        foreach ( Client client in Client.All )
        {
            PlayerInfo.UpdateGameState( client, PlayerGameState.Spectating );

            if( client.Camera != null ) client.Camera = null;

            if ( client.Pawn != null || client.Pawn.IsValid() ) client.Pawn.Delete();

            var player = new BRPlayer();
            client.Pawn = player;
            player.Respawn();
        }
    }

    [ClientRpc]
    public void SendStartWaiting()
    {
        CurrentState = GameState.Waiting;
    }

    public void StartStarting()
    {
        StartingTime = 0;
        CurrentState = GameState.Starting;

        SendStartStarting( To.Everyone, StartingTime );
    }

    [ClientRpc]
    public void SendStartStarting( TimeSince startTime )
    {
        StartingTime = startTime;
        CurrentState = GameState.Starting;
    }

    public void StartGame()
    {
        ZoneTicks = 0;
        CurrentState = GameState.Active;

        SendStartGame( To.Everyone );

        foreach ( Client client in Client.All )
        {
            if ( client.Pawn == null || !client.Pawn.IsValid() ) continue;
            PlayerInfo.UpdateGameState( client.Pawn as Player, PlayerGameState.Alive );

            if ( IsServer ) (client.Pawn as Player).Respawn();
        }

        if ( IsServer )
        {
            foreach ( var data in SupplyCrate.SpawnLocations )
            {
                new SupplyCrate
                {
                    Position = data.Item1,
                    WorldAng = data.Item2
                };
            }
        }
    }

    [ClientRpc]
    public void SendStartGame( int zoneTicks = 0 )
    {
        ZoneTicks = zoneTicks;
        CurrentState = GameState.Active;
    }

    public void EndGame()
    {
        EndedTime = 0;
        CurrentState = GameState.Ended;

        SendEndGame( To.Everyone, 0 );

        if ( IsServer )
        {
            foreach ( Entity ent in All )
            {
                if ( ent is not FloorUsable usableEnt ) continue;
                usableEnt.Delete();
            }

            foreach( var kv in PlayerInfo.Players )
            {
                PlayerInfo playerInfo = kv.Value;
                if ( playerInfo.State == PlayerGameState.Spectating ) continue;

                PlayerData.AddStats( playerInfo.Client, playerInfo.Kills, playerInfo.State == PlayerGameState.Alive ? 1 : 0, (int)playerInfo.Survived );
            }
        }
    }

    [ClientRpc]
    public void SendEndGame( TimeSince endTime )
    {
        EndedTime = endTime;
        CurrentState = GameState.Ended;
    }

    public override void ClientJoined( Client client )
    {
        base.ClientJoined( client );

        PlayerInfo.AddPlayer( client );
        PlayerInfo.UpdateGameState( client, PlayerGameState.Spectating );

        SendGameState( To.Single( client ), CurrentState );

        if ( CurrentState == GameState.Waiting || CurrentState == GameState.Starting ) 
            DelayedClients.Add( client, 0 );
    }

    [ClientRpc]
    private void SendGameState( GameState currentState )
    {
        CurrentState = currentState;

        switch( currentState )
        {
            case GameState.Waiting:
                SendStartWaiting();
                break;         
            case GameState.Starting:
                SendStartStarting( StartingTime );
                break;            
            case GameState.Active:
                SendStartGame( ZoneTicks );
                break;          
            case GameState.Ended:
                SendEndGame( EndedTime );
                break;
        }
    }

    [Event( "server.tick" )]
    private void ClientJoinDelay()
    {
        List<Client> toDelete = new();
        foreach( var kv in DelayedClients )
        {
            if ( kv.Value < .5f ) continue;
            Client client = kv.Key;

            if( client == null || !client.IsValid() )
            {
                toDelete.Add( client );
                continue;
            }

            if ( CurrentState == GameState.Waiting || CurrentState == GameState.Starting )
            {
                var player = new BRPlayer();
                client.Pawn = player;
                player.Respawn();
            } else
            {
                client.Camera = new BRSpectateCamera();
            }

            toDelete.Add( client );
        }

        foreach( Client client in toDelete )
        {
            DelayedClients.Remove( client );
        }
    }

    public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
    {
        base.ClientDisconnect( cl, reason );

        PlayerInfo.RemovePlayer( cl );
    }

    public static bool IsSpectating()
    {
        if ( !Host.IsClient ) return false;
        if ( Local.Client == null || PlayerInfo.GetPlayerInfo( Local.Client ) is not PlayerInfo playerInfo ) return true;

        return playerInfo.State != PlayerGameState.Alive && (BRGame.CurrentState == GameState.Active || BRGame.CurrentState == GameState.Ended);
    }

    public override void MoveToSpawnpoint( Entity pawn )
    {
        var spawnpoints = All.OfType<SpawnPoint>().OrderBy( x => Guid.NewGuid() );
        var players = All.OfType<Player>();

        SpawnPoint spawnpoint = null;
        foreach( SpawnPoint point in spawnpoints )
        {
            bool foundPlayer = false;
            foreach( Player player in players )
            {
                if( player.Position.Distance( point.Position ) < 500 )
                {
                    foundPlayer = true;
                    break;
                }
            }

            if ( foundPlayer ) continue;

            spawnpoint = point;
            break;
        }

        if ( spawnpoint == null ) spawnpoint = All.OfType<SpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

        if ( spawnpoint == null )
        {
            Log.Warning( $"Couldn't find spawnpoint for {pawn}!" );
            return;
        }

        pawn.Transform = spawnpoint.Transform;
    }

    [ServerCmd( "kill" )]
    public static void KillOverride()
    {

    }
}

public enum GameState
{
    Waiting,
    Starting,
    Active,
    Ended
}
