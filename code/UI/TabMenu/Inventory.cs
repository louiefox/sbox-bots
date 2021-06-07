
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace BattleRoyale.UI
{
    public class Inventory : Panel
    {
        private IDictionary<int, InventorySlotBack> Slots { get; set; } = new Dictionary<int, InventorySlotBack>();
        private Panel RowContainer;

        public Inventory()
        {
            StyleSheet.Load( "/ui/TabMenu/Inventory.scss" );

            RowContainer = Add.Panel( "rowcontainer" );
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
                RowContainer.DeleteChildren();

                Panel currentRow = null;
                int rowSlots = 0;

                for ( int i = 1; i <= inventory.MaxSlots; i++ )
                {
                    if ( currentRow == null || rowSlots >= 6 )
                    {
                        currentRow = RowContainer.Add.Panel( "row" );
                        rowSlots = 0;
                    }

                    rowSlots++;

                    InventorySlotBack slot = currentRow.AddChild<InventorySlotBack>( "slotback" );

                    if ( rowSlots == 6 )
                    {
                        slot.Style.MarginRight = 0f;
                    }

                    Slots.Add( i, slot );
                }
            }

            for ( int i = 1; i <= inventory.MaxSlots; i++ )
            {
                if ( !Slots.ContainsKey( i ) ) continue;

                if ( !inventory.Slots.ContainsKey( i ) )
                {
                    Slots[i].Slot?.Delete();
                    Slots[i].Slot = null;
                    continue;
                }

                BRInventoryItem itemData = inventory.Slots[i];
                LootItem item = LootItem.Items[itemData.ItemID];

                if( Slots[i].Slot == null )
                {
                    Slots[i].Slot = Slots[i].AddChild<InventorySlot>( "slot" );
                }

                InventorySlot slot = Slots[i].Slot;
                slot.SlotKey = i;
                slot.Name.Text = item.Name;
                slot.Amount.Text = $"x{itemData.Amount}";
                slot.ChangeRarity( item.Rarity );
                //slot.SetModel( item.Model );
            }
        }
    }

    public class InventorySlotBack : Panel
    {
        public InventorySlot Slot;
    }

    public class InventorySlot : Panel
    {
        public int SlotKey;
        public Label Name;
        public Label Amount;

        private ModelDisplay Model;
        private ItemRarity CurrentRarity;

        public InventorySlot()
        {
            Name = Add.Label( "", "name" );
            Amount = Add.Label( "x0", "amount" );

            //Model = AddChild<ModelDisplay>();
            //Model.Style.Set( "width: 100%; height: 100%;" );

            AddEvent( "onclick", () => {

            } );

            AddEvent( "onrightclick", () => {
                if ( Local.Pawn is not BRPlayer player || !player.ItemInventory.Slots.ContainsKey( SlotKey ) ) return;
                ConsoleSystem.Run( "request_inventory_drop", SlotKey );
            } );
        }

        public void ChangeRarity( ItemRarity rarity )
        {
            RemoveClass( CurrentRarity.ToString().ToLower() );

            CurrentRarity = rarity;
            AddClass( rarity.ToString().ToLower() );
        }

        public void SetModel( string model )
        {
            Model.UpdateModel( model );
        }
    }
}
