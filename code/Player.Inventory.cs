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

        Event.Run("battleroyale.updateinv");
    }
}