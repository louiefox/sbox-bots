using Sandbox;
using BattleRoyale;

[Library( "bots" )]
public partial class BRGame : Sandbox.Game
{
    BRHud BRHUD;
    public BRGame()
    {
        //
        // Create the HUD entity. This is always broadcast to all clients
        // and will create the UI panels clientside. It's accessible 
        // globally via Hud.Current, so we don't need to store it.
        //
        if ( IsClient )
        {
            BRHUD = new BRHud();
        }
    }

    [Event.Hotload]
    public void UpdateHUD()
    {
        if ( !IsClient || BRHUD == null ) { return; }
        BRHUD.Delete();
        BRHUD = new BRHud();
    }

    public override void ClientJoined( Client client )
    {
        base.ClientJoined( client );

        // Create a pawn and assign it to the client.
        var player = new BRPlayer();
        client.Pawn = player;

        player.Respawn();

        PlayerInfo.AddPlayer( client );
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
