
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
            BRPlayer player = Local.Pawn as BRPlayer;
            BRInventory inventory = player.Inventory;

            Panel slotContainer = Add.Panel( "slotcontainer" );

            for ( int i = 1; i <= inventory.MaxSlots; i++ )
            {
                Panel slot = slotContainer.Add.Panel( "slot" );
                slot.Add.Label( i.ToString() );

                Slots.Add( i, slot );
            }
        }
    }
}
