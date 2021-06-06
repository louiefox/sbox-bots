using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using BattleRoyale;

partial class BRPlayer
{
    [ClientRpc]
    public void CLAddInventoryItem(int slot, string itemID, int amount)
    {
        ItemInventory.Slots[slot] = new BRInventoryItem(itemID, amount);

        Event.Run("battleroyale.updateinv");
    }

    [ClientRpc]
    public void CLRemoveInventoryItem(int slot)
    {
        ItemInventory.Slots.Remove(slot);

        Event.Run( "battleroyale.updateinv" );
    }

    [ServerCmd( "request_inventory_drop" )]
    public static void RequestDrop( int slot )
    {
        var ply = ConsoleSystem.Caller.Pawn as BRPlayer;

        if ( ply == null || ply.LifeState != LifeState.Alive || ply.ItemInventory is not BRInventory inventory || !inventory.Slots.ContainsKey( slot ) ) return;

        inventory.Drop( slot, ply.Position + ply.EyeRot.Forward * 20 );
    }
}
