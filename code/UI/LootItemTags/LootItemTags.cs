using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sandbox.UI
{
    public class BaseLootItemTag : Panel
    {
        public Label NameLabel;
        public Label TypeLabel;
        public LootPickup LootEnt;

        private ItemRarity CurrentRarity;

        public BaseLootItemTag( LootPickup lootEnt )
        {
            LootEnt = lootEnt;

            Panel infoPanel = Add.Panel( "infoback" );
            NameLabel = infoPanel.Add.Label( "", "name" );
            TypeLabel = infoPanel.Add.Label( "", "type" );

            Panel keyPanel = Add.Panel( "keyback" );
            keyPanel.Add.Label( "E", "key" );

            ChangeRarity( ItemRarity.Common );
        }

        public override void Tick()
        {
            base.Tick();

            LootItem item = LootItem.Items[LootEnt.ItemID];

            NameLabel.Text = LootEnt.Amount > 1 ? $"x{LootEnt.Amount} {item.Name}" : $"{item.Name}";
            TypeLabel.Text = $"{item.Type}";

            if ( CurrentRarity != item.Rarity )
            {
                ChangeRarity( item.Rarity );
            }
        }

        private void ChangeRarity( ItemRarity rarity )
        {
            RemoveClass( CurrentRarity.ToString().ToLower() );

            CurrentRarity = rarity;
            AddClass( rarity.ToString().ToLower() );
        }
    }

    public class LootItemTags : Panel
    {
        private LootPickup CurrentTarget;
        private BaseLootItemTag Tag;

        public float MaxDrawDistance = 400;

        public LootItemTags()
        {
            StyleSheet.Load( "/ui/lootitemtags/LootItemTags.scss" );
        }

        public override void Tick()
        {
            base.Tick();

            CurrentTarget = BRPlayer.GetNewTargetLoot() as LootPickup;

            if ( (CurrentTarget == null || !CurrentTarget.IsValid()) && Tag != null )
            {
                Tag.Delete();
                Tag = null;
                return;
            }

            if ( CurrentTarget == null || !CurrentTarget.IsValid() ) return;

            if ( Tag == null )
            {
                Tag = new BaseLootItemTag( CurrentTarget );
                Tag.Parent = this;
            }

            if ( Tag.LootEnt != CurrentTarget )
            {
                Tag.LootEnt = CurrentTarget;
            }

            UpdateTag();
        }

        public void UpdateTag()
        {
            var labelPos = CurrentTarget.Position + new Vector3( 0, 0, 20f );

            float dist = labelPos.Distance( CurrentView.Position );
            if ( dist > MaxDrawDistance )
            {
                Tag.Style.Opacity = 0;
                Tag.Style.Dirty();
                return;
            }

            var screenPos = labelPos.ToScreen();

            Tag.Style.Opacity = 1;
            Tag.Style.Left = Length.Fraction( screenPos.x );
            Tag.Style.Top = Length.Fraction( screenPos.y );

            var transform = new PanelTransform();
            transform.AddTranslateY( Length.Fraction( -1.0f ) );
            transform.AddScale( 1 );
            transform.AddTranslateX( Length.Fraction( -0.5f ) );

            Tag.Style.Transform = transform;
            Tag.Style.Dirty();
        }
    }
}
