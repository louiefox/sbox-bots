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

			NameLabel.Text = $"{item.Name}";
			TypeLabel.Text = $"{item.Type}";

			if( CurrentRarity != item.Rarity )
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
		private BaseLootItemTag CurrentTag;

		public float MaxDrawDistance = 400;

		public LootItemTags()
		{
			StyleSheet.Load( "/ui/lootitemtags/LootItemTags.scss" );
		}

		public override void Tick()
		{
			base.Tick();

			CurrentTarget = (Local.Pawn as BRPlayer).GetNewTargetLoot() as LootPickup;

			if( CurrentTarget == null && CurrentTag != null )
			{
				CurrentTag.Delete();
				CurrentTag = null;
				return;
			}

			if ( CurrentTarget == null ) return;

			if( CurrentTag == null )
			{
				CurrentTag = CreateNameTag( CurrentTarget );
			}

			if( CurrentTag.LootEnt != CurrentTarget )
			{
				CurrentTag.LootEnt = CurrentTarget;
			}

			UpdateTag();
		}

		public virtual BaseLootItemTag CreateNameTag( LootPickup lootEnt )
		{
			var tag = new BaseLootItemTag( lootEnt );
			tag.Parent = this;
			return tag;
		}

		public void UpdateTag()
		{
			var labelPos = CurrentTarget.Position + new Vector3( 0, 0, 20f );

			float dist = labelPos.Distance( CurrentView.Position );
			if ( dist > MaxDrawDistance ) return;

			var alpha = dist.LerpInverse( MaxDrawDistance, MaxDrawDistance * 0.1f, true );
			var screenPos = labelPos.ToScreen();

			var tag = CurrentTag;

			tag.Style.Left = Length.Fraction( screenPos.x );
			tag.Style.Top = Length.Fraction( screenPos.y );
			//tag.Style.Opacity = alpha;

			var transform = new PanelTransform();
			transform.AddTranslateY( Length.Fraction( -1.0f ) );
			transform.AddScale( 1 );
			transform.AddTranslateX( Length.Fraction( -0.5f ) );

			tag.Style.Transform = transform;
			tag.Style.Dirty();
		}
	}
}
