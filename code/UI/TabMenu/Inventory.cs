
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace BattleRoyale.UI
{
    public class Inventory : Panel
    {
        private IDictionary<int, Panel> Slots { get; set; } = new Dictionary<int, Panel>();

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
                    int slotKey = i;

                    if ( currentRow == null || rowSlots >= 6 )
                    {
                        currentRow = rowContainer.Add.Panel( "row" );
                        rowSlots = 0;
                    }

                    rowSlots++;

                    InventorySlot slot = currentRow.AddChild<InventorySlot>( "slotback" );

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

                Slots[i].DeleteChildren();

                if ( !inventory.Slots.ContainsKey( i ) ) continue;

                BRInventoryItem itemData = inventory.Slots[i];
                LootItem item = LootItem.Items[itemData.ItemID];

                InventorySlot slot = Slots[i].AddChild<InventorySlot>( "slot" );
                slot.SlotKey = i;
                slot.Name.Text = item.Name;
                slot.Amount.Text = $"x{itemData.Amount}";
                slot.ChangeRarity( item.Rarity );
                //slot.SetModel( item.Model );
            }
        }
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
