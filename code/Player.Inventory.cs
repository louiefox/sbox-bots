using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using BattleRoyale;

partial class BRPlayer
{
    public int GetInvItemCount( string itemID )
    {
        int totalCount = 0;
        foreach( var data in ItemInventory.Slots )
        {
            if ( data.Value.ItemID != itemID ) continue;
            totalCount += data.Value.Amount;
        }

        return totalCount;
    }    
    
    public int TakeInvItems( string itemID, int amount )
    {
        int totalTaken = 0;
        foreach( var data in ItemInventory.Slots )
        {
            if ( data.Value.ItemID != itemID ) continue;

            int amountToTake = Math.Min( data.Value.Amount, amount-totalTaken );
            ItemInventory.TakeAmount( data.Key, amountToTake );

            totalTaken += amountToTake;

            if ( totalTaken == amount ) break;
        }

        return totalTaken;
    }

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

    [ClientRpc]
    public void CLTakeInventoryItemAmount(int slot, int amount)
    {
        BRInventoryItem itemData = ItemInventory.Slots[slot];
        itemData.Amount = amount;
        ItemInventory.Slots[slot] = itemData;

        Event.Run( "battleroyale.updateinv" );
    }        
    
    [ClientRpc]
    public void CLUpdateInventoryItem( int slot, string itemID, int amount )
    {
        ItemInventory.Slots[slot] = new BRInventoryItem( itemID, amount );

        Event.Run( "battleroyale.updateinv" );
    }     
    
    [ServerCmd( "request_inventory_drop" )]
    public static void RequestDrop( int slot )
    {
        if ( BRGame.CurrentState != GameState.Active ) return;
        var ply = ConsoleSystem.Caller.Pawn as BRPlayer;

        if ( ply == null || ply.LifeState != LifeState.Alive || ply.ItemInventory is not BRInventory inventory || !inventory.Slots.ContainsKey( slot ) ) return;

        inventory.Drop( slot, ply.Position + ply.EyeRot.Forward * 20 );
    }
}
