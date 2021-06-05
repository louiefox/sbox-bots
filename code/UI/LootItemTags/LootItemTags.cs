﻿using Sandbox.UI.Construct;
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
		private BaseLootItemTag Tag;

		public float MaxDrawDistance = 400;

		public LootItemTags()
		{
			StyleSheet.Load( "/ui/lootitemtags/LootItemTags.scss" );
		}

		public override void Tick()
		{
			base.Tick();

			CurrentTarget = (Local.Pawn as BRPlayer).GetNewTargetLoot() as LootPickup;

			if( (CurrentTarget == null || !CurrentTarget.IsValid()) && Tag != null )
			{
				Tag.Delete();
				Tag = null;
				return;
			}

			if ( CurrentTarget == null || !CurrentTarget.IsValid() ) return;

			if( Tag == null )
			{
				Tag = CreateNameTag( CurrentTarget );
			}

			if( Tag.LootEnt != CurrentTarget )
			{
				Tag.LootEnt = CurrentTarget;
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

			Tag.Style.Left = Length.Fraction( screenPos.x );
			Tag.Style.Top = Length.Fraction( screenPos.y );
			//tag.Style.Opacity = alpha;

			var transform = new PanelTransform();
			transform.AddTranslateY( Length.Fraction( -1.0f ) );
			transform.AddScale( 1 );
			transform.AddTranslateX( Length.Fraction( -0.5f ) );

			Tag.Style.Transform = transform;
			Tag.Style.Dirty();
		}
	}
}
