using Sandbox;
using BattleRoyale;
using System.Collections.Generic;

[Library( "bots", Title = "Battle of Terrys" )]
public partial class BRGame : Sandbox.Game
{
    public static GameState CurrentState = GameState.Waiting;
    public static TimeSince StartingTime;
    public static TimeSince EndedTime;
    public static float StartDuration = 5f;
    public static float EndDuration = 5f;

    private BattleRoyaleHUD BattleRoyaleHUD;
    private Dictionary<Client, TimeSince> DelayedClients = new();

    public BRGame()
    {
        if ( IsClient ) BattleRoyaleHUD = new BattleRoyaleHUD();
    }

    [Event.Hotload]
    public void UpdateHUD()
    {
        if ( !IsClient || BattleRoyaleHUD == null ) { return; }
        BattleRoyaleHUD.Delete();
        BattleRoyaleHUD = new BattleRoyaleHUD();
    }

    [Event.Tick]
    public void GameTick()
    {
        if ( CurrentState == GameState.Waiting )
        {
            if ( Client.All.Count > 1 )
            {
                StartingTime = 0;
                CurrentState = GameState.Starting;
            }
        }
        else if ( CurrentState == GameState.Starting )
        {
            if ( Client.All.Count <= 1 ) CurrentState = GameState.Waiting;

            if ( StartingTime >= StartDuration ) StartGame();
        }
        else if ( CurrentState == GameState.Active )
        {
            int alivePlayers = 0;
            foreach( var kv in PlayerInfo.Players )
            {
                if ( kv.Value.State != PlayerGameState.Alive ) continue;
                alivePlayers++;
            }    

            if ( Client.All.Count <= 1 || alivePlayers <= 1 ) EndGame();
        }        
        else if ( CurrentState == GameState.Ended )
        {
            if ( EndedTime > EndDuration ) StartWaiting();
        }
    }

    public void StartWaiting()
    {
        CurrentState = GameState.Waiting;

        foreach ( Client client in Client.All )
        {
            PlayerInfo.UpdateGameState( client, PlayerGameState.Spectating );

            if( client.Camera != null )
            {
                client.Camera = null;
            }

            if ( !IsServer ) continue;

            if ( client.Pawn != null || client.Pawn.IsValid() ) client.Pawn.Delete();

            var player = new BRPlayer();
            client.Pawn = player;
            player.Respawn();
        }
    }

    public void StartGame()
    {
        CurrentState = GameState.Active;

        foreach ( Client client in Client.All )
        {
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

        Log.Info( "Game Started" );
    }    
    
    public void EndGame()
    {
        EndedTime = 0;
        CurrentState = GameState.Ended;

        if( IsServer )
        {
            foreach ( Entity ent in All )
            {
                if ( ent is not FloorUsable usableEnt ) continue;
                usableEnt.Delete();
            }
        }

        Log.Info( "Game Ended" );
    }

    public override void ClientJoined( Client client )
    {
        base.ClientJoined( client );

        PlayerInfo.AddPlayer( client );
        PlayerInfo.UpdateGameState( client, PlayerGameState.Spectating );

        if( CurrentState == GameState.Waiting || CurrentState == GameState.Starting ) 
            DelayedClients.Add( client, 0 );
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

            var player = new BRPlayer();
            client.Pawn = player;
            player.Respawn();

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
        return PlayerInfo.GetPlayerInfo( Local.Client ).State != PlayerGameState.Alive && (BRGame.CurrentState == GameState.Active || BRGame.CurrentState == GameState.Ended);
    }
}

public enum GameState
{
    Waiting,
    Starting,
    Active,
    Ended
}
