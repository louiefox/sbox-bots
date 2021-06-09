using Sandbox;
using BattleRoyale;
using System.Collections.Generic;

[Library( "bots", Title = "Battle of Terrys" )]
public partial class BRGame : Sandbox.Game
{
    public static GameState CurrentState = GameState.Waiting;
    public static TimeSince StartingTime;
    public static TimeSince EndedTime;
    public static float StartDuration = 10f;
    public static float EndDuration = 15f;

    private BRHud BRHUD;

    public BRGame()
    {
        if ( IsClient ) BRHUD = new BRHud();
    }

    [Event.Hotload]
    public void UpdateHUD()
    {
        if ( !IsClient || BRHUD == null ) { return; }
        BRHUD.Delete();
        BRHUD = new BRHud();
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
            if ( EndedTime > EndDuration ) CurrentState = GameState.Waiting;
        }
    }

    public void StartGame()
    {
        CurrentState = GameState.Active;

        foreach ( Client client in Client.All )
        {
            PlayerInfo.UpdateGameState( client.Pawn as Player, PlayerGameState.Alive );
        }

        Log.Info( "Game Started" );
    }    
    
    public void EndGame()
    {
        EndedTime = 0;
        CurrentState = GameState.Ended;

        foreach ( Client client in Client.All )
        {
            PlayerInfo.UpdateGameState( client.Pawn as Player, PlayerGameState.Spectating );
        }

        Log.Info( "Game Ended" );
    }

    Dictionary<Client, TimeSince> DelayedClients = new();
    public override void ClientJoined( Client client )
    {
        base.ClientJoined( client );

        DelayedClients.Add( client, 0 );
    }

    [Event.Tick]
    private void ClientJoinDelay()
    {
        List<Client> toDelete = new();
        foreach( var kv in DelayedClients )
        {
            if ( kv.Value < 2 ) return;
            Log.Info( "Delayed client loaded" );

            Client client = kv.Key;

            var player = new BRPlayer();
            client.Pawn = player;
            player.Respawn();

            PlayerInfo.AddPlayer( client );
            PlayerInfo.UpdateGameState( player, PlayerGameState.Spectating );

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

    [ServerCmd( "test_spawnloot" )]
    public static void TestSpawnLoot( string itemID )
    {
        var owner = ConsoleSystem.Caller.Pawn;

        if ( owner == null )
            return;

        var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
            .UseHitboxes()
            .Ignore( owner )
            .Size( 2 )
            .Run();

        LootPickup lootEnt = new LootPickup
        {
            Position = tr.EndPos + new Vector3( 0, 0, 20f )
        };

        lootEnt.SetItem( itemID );
    }

    [ServerCmd( "test_spawncrate" )]
    public static void TestSpawnCrate()
    {
        var owner = ConsoleSystem.Caller.Pawn;

        if ( owner == null )
            return;

        var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
            .UseHitboxes()
            .Ignore( owner )
            .Run();

        new SupplyCrate
        {
            Position = tr.EndPos
        };
    }
}

public enum GameState
{
    Waiting,
    Starting,
    Active,
    Ended
}
