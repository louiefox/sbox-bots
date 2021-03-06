using Sandbox;
using BattleRoyale;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

public partial class BRGame
{
	[ServerCmd( "test_spawnheli" )]
    public static void TestSpawnHeli()
    {
		DropShip dropShip = new();
		dropShip.StartMoving();

		foreach( Client client in Client.All )
		{
			client.Camera = dropShip.DropCamera;
		}
	}	
	
	[ServerCmd( "test_spawnloot" )]
    public static void TestSpawnLoot( string itemID, int amount )
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

        lootEnt.SetItem( itemID, amount );
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
	
	[ServerCmd( "test_dress" )]
    public static void TestDress()
    {
        var owner = ConsoleSystem.Caller.Pawn as BRPlayer;

        if ( owner == null )
            return;

        owner.Dress();
	}        
}
