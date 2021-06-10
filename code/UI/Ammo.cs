
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace BattleRoyale.UI
{
    public class Ammo : Panel
    {
	    public Label Weapon;
	    public Label Inventory;
        private Panel AmmoRow;
        private List<Panel> AmmoIcons = new();

	    public Ammo()
	    {
            StyleSheet.Load( "/ui/Ammo.scss" );

            Panel textRow = Add.Panel( "textrow" );
            Weapon = textRow.Add.Label( "100", "weapon" );
		    Inventory = textRow.Add.Label( "100", "inventory" );

            AmmoRow = Add.Panel( "ammorow" );
        }

        private void UpdateAmmoIcons( int amount )
        {
            int diff = AmmoIcons.Count - amount;
            if ( amount > AmmoIcons.Count )
            {
                foreach( Panel panel in AmmoIcons )
                {
                    panel.Delete( true );
                }

                AmmoIcons = new();

                for ( int i = 0; i < amount; i++ )
                {
                    Panel ammoIcon = AmmoRow.Add.Panel( "ammoicon" );

                    AmmoIcons.Add( ammoIcon );
                }
            } else if ( amount < AmmoIcons.Count )
            {
                while( amount < AmmoIcons.Count )
                {
                    int index = AmmoIcons.Count - 1;

                    Panel panel = AmmoIcons[index];
                    panel.Delete( true );

                    if( diff == 1 ) new AmmoIcon( Parent );

                    AmmoIcons.RemoveAt( index );
                }
            }
        }

	    public override void Tick()
	    {
		    var player = Local.Pawn;
		    if ( player == null ) return;

		    var weapon = player.ActiveChild as BaseBRWeapon;
		    SetClass( "active", weapon != null );

		    if ( weapon == null ) return;

            if( weapon.AmmoClip != AmmoIcons.Count )
            {
                UpdateAmmoIcons( weapon.AmmoClip );
            }

		    Weapon.Text = $"{weapon.AmmoClip}";

		    var inv = weapon.AvailableAmmo();
		    Inventory.Text = $" / {inv}";
		    Inventory.SetClass( "active", inv >= 0 );
	    }
    }

    public class AmmoIcon : Panel
    {
        private float CreatedAt;

        public AmmoIcon( Panel parent )
        {
            StyleSheet.Load( "/ui/Ammo.scss" );

            Parent = parent;

            CreatedAt = Time.Now;
        }

        public override void Tick()
        {
            SetClass( "ammomoveright", Time.Now >= CreatedAt + .01f );
            SetClass( "ammofalling", Time.Now >= CreatedAt + .11f );

            if ( Time.Now >= CreatedAt + .4f )
            {
                Delete( true );
            }
        }
    }
}
