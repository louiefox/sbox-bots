using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BattleRoyale
{
    public partial class BRInventory
    {
        public Entity Owner { get; init; }
        public IDictionary<int, BRInventoryItem> Slots { get; set; } = new Dictionary<int, BRInventoryItem>();

        public int MaxSlots { get; set; } = 12;

        public BRInventory( Entity owner )
        {
            Owner = owner;
        }

        public bool Add( BRInventoryItem data, int slot )
        {
            if ( Slots.ContainsKey( slot ) || slot > MaxSlots ) return false;

            Slots[slot] = data;

            BRPlayer player = Owner as BRPlayer;
            player.CLAddInventoryItem( To.Single( Owner.GetClientOwner() ), slot, data.ItemID, data.Amount );

            return true;
        }

        public int Add( BRInventoryItem data )
        {
            int maxStack = LootItem.Items[data.ItemID].MaxStack;
            if ( maxStack > 1 || data.Amount > maxStack )
            {
                int remainingAmount = data.Amount;
                for ( int i = 1; i <= MaxSlots; i++ )
                {
                    if ( Slots.ContainsKey( i ) )
                    {
                        BRInventoryItem itemData = Slots[i];
                        if ( itemData.ItemID == data.ItemID && itemData.Amount < maxStack )
                        {
                            int addAmount = Math.Min( maxStack - itemData.Amount, remainingAmount );

                            if ( !UpdateSlotAmount( i, itemData.Amount + addAmount ) ) continue;
                            remainingAmount -= addAmount;

                            if ( remainingAmount <= 0 ) break;
                        }

                        continue;
                    }

                    int amount = Math.Min( remainingAmount, maxStack );
                    remainingAmount -= amount;

                    Add( new BRInventoryItem( data.ItemID, amount ), i );
                    if ( remainingAmount <= 0 ) break;
                }

                if ( remainingAmount <= 0 ) return data.Amount;
                return data.Amount - remainingAmount;
            }

            return Add( data, GetNewSlot() ) ? data.Amount : 0;
        }

        public bool UpdateSlotAmount( int slot, int amount )
        {
            if ( !Slots.ContainsKey( slot ) || slot > MaxSlots ) return false;

            Slots[slot] = new BRInventoryItem( Slots[slot].ItemID, amount );

            BRPlayer player = Owner as BRPlayer;
            player.CLUpdateInventoryItem( To.Single( Owner.GetClientOwner() ), slot, Slots[slot].ItemID, amount );

            return true;
        }

        public int GetNewSlot()
        {
            int newIndex = 0;
            for ( int i = 1; i <= Slots.Count; i++ )
            {
                if ( Slots.ContainsKey( i ) ) continue;

                newIndex = i;
                break;
            }

            return newIndex >= 1 ? newIndex : Slots.Count + 1;
        }

        public bool Remove( int slot )
        {
            if ( !Slots.ContainsKey( slot ) ) return false;

            Slots.Remove( slot );

            BRPlayer player = Owner as BRPlayer;
            player.CLRemoveInventoryItem( To.Single( Owner.GetClientOwner() ), slot );

            return true;
        }

        public Entity Drop( int slot, Vector3 pos )
        {
            if ( !Slots.ContainsKey( slot ) ) return null;

            BRInventoryItem data = Slots[slot];
            Remove( slot );

            LootPickup lootEnt = new();
            lootEnt.SetPosition( pos );
            lootEnt.SetItem( data.ItemID, data.Amount );

            LootItem item = LootItem.Items[data.ItemID];

            foreach ( var kv in FloorUsable.IndexEnts )
            {
                if ( kv.Value is LootPickup ent && ent != lootEnt && data.ItemID == ent.ItemID && lootEnt.Position.Distance( ent.Position ) < 60f )
                {
                    item.CombineItem( ent, lootEnt );
                    break;
                }
            }

            return lootEnt;
        }

        public Entity Drop( int slot )
        {
            return Drop( slot, Owner.Position );
        }

        public void TakeAmount( int slot, int amount )
        {
            if ( !Slots.ContainsKey( slot ) ) return;

            BRInventoryItem itemData = Slots[slot];
            itemData.Amount -= amount;
            Slots[slot] = itemData;

            if ( itemData.Amount > 0 )
            {
                BRPlayer player = Owner as BRPlayer;
                player.CLTakeInventoryItemAmount( To.Single( Owner.GetClientOwner() ), slot, itemData.Amount );
            } else
            {
                Remove( slot );
            }
        }
    }

    public struct BRInventoryItem
    {
        public string ItemID { get; set; }
        public int Amount { get; set; }

        public BRInventoryItem( string itemID, int amount )
        {
            ItemID = itemID;
            Amount = amount;
        }
    }
}
