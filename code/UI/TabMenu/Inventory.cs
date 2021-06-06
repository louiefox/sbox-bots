
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using BattleRoyale;

namespace BattleRoyale.UI
{
    public class Inventory : Panel
    {
        private IDictionary<int, InventorySlot> Slots { get; set; } = new Dictionary<int, InventorySlot>();

        public Inventory()
        {
            StyleSheet.Load( "/ui/TabMenu/Inventory.scss" );

            UpdateInventory();
        }

        /// <summary>
        ///     I am not conforming to your standards Alex.
        /// </summary>
        [Event( "battleroyale.updateinv" )]
        public void UpdateInventory()
        {
            if ( Local.Pawn is not BRPlayer player ) return;

            BRInventory inventory = player.ItemInventory;

            if ( Slots.Count != inventory.MaxSlots )
            {
                Panel rowContainer = Add.Panel( "rowcontainer" );

                Panel currentRow = null;
                int rowSlots = 0;

                for ( int i = 1; i <= inventory.MaxSlots; i++ )
                {
                    if ( currentRow == null || rowSlots >= 6 )
                    {
                        currentRow = rowContainer.Add.Panel( "row" );
                        rowSlots = 0;
                    }

                    rowSlots++;

                    InventorySlot slot = currentRow.AddChild<InventorySlot>( "slot" );

                    Slots.Add( i, slot );
                }
            }

            foreach ( var kv in inventory.Slots )
            {
                if ( !Slots.ContainsKey( kv.Key ) ) continue;

                LootItem item = LootItem.Items[kv.Value.ItemID];

                InventorySlot slot = Slots[kv.Key];
                slot.Name.Text = item.Name;
            }
        }
    }

    public class InventorySlot : Panel
    {
        public Label Name;

        public InventorySlot()
        {
            Name = Add.Label( "Test", "name" );
        }
    }
}
