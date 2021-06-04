using Sandbox;
using System;
using System.Collections.Generic;

[Library( "ent_loot_pickup" )]
public partial class LootPickup : Prop
{
	private static Dictionary<int, LootPickup> IndexEnts = new Dictionary<int, LootPickup>();

	[Net]
	public int Index { get; set; }

	[Net]
	public string ItemID { get; set; }

	public Prop ClientModel;
	public float CurrentZ = 0f;

	public override void Spawn()
	{
		base.Spawn();

		if ( !IsServer ) return;

		int newIndex = -1;
		for ( int i = 0; i < IndexEnts.Count; i++ )
		{
			if ( IndexEnts.ContainsKey( i ) ) continue;

			newIndex = i;
			break;
		}

		Index = newIndex >= 0 ? newIndex : IndexEnts.Count;
		IndexEnts.Add( Index, this );
	}

	public void SetItem(string itemID)
	{
		ItemID = itemID;

		LootItem item = LootItem.Items[ItemID];

		//SetModel( item.Model );
		//SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );

		CreateClientModel();
	}

	[ClientRpc]
	public void CreateClientModel()
	{
		LootItem item = LootItem.Items[ItemID];

		Color32 glowColor;
		switch (item.Rarity)
		{
			case ItemRarity.Uncommon:
				glowColor = new Color32( 75, 235, 61 );
				break;
			case ItemRarity.Rare:
				glowColor = new Color32( 67, 61, 235 );
				break;			
			case ItemRarity.Epic:
				glowColor = new Color32( 154, 61, 235 );
				break;			
			case ItemRarity.Legendary:
				glowColor = new Color32( 235, 177, 61 );
				break;
			default:
				glowColor = new Color32( 176, 176, 176 );
				break;
		}

		ClientModel = new Prop();
		ClientModel.Parent = this;
		ClientModel.Position = Position;
		ClientModel.SetModel( item.Model );

		ClientModel.GlowState = GlowStates.GlowStateOff;
		ClientModel.GlowDistanceStart = 0;
		ClientModel.GlowDistanceEnd = 1000;
		ClientModel.GlowColor = glowColor.ToColor();
		ClientModel.GlowActive = false;
	}

	[Event( "client.tick" )]
	public void UpdateClientModel()
	{
		if ( ClientModel == null || !ClientModel.IsValid() ) return;

		CurrentZ = CurrentZ + 1f;

		ClientModel.Position = Position;
		ClientModel.Rotation = Rotation.LookAt( Local.Pawn.Position );
	}

	public void EnableGlow()
	{
		ClientModel.GlowState = GlowStates.GlowStateOn;
		ClientModel.GlowActive = true;
	}	
	
	public void DisableGlow()
	{
		ClientModel.GlowState = GlowStates.GlowStateOff;
		ClientModel.GlowActive = false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		ClientModel?.Delete();
		IndexEnts.Remove( Index );
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	[ServerCmd( "request_loot_pickup" )]
	public static void RequestPickup(int index)
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
		if ( owner == null )
			return;

		if ( owner is BRPlayer ply && IndexEnts.ContainsKey( index ) )
		{
			LootPickup ent = IndexEnts[index];
			if ( ent == null || !ent.IsValid() ) return;

			var pickupEffect = Particles.Create( "particles/money_pickup.vpcf" );
			pickupEffect.SetPos( 0, ent.Position );

			string itemID = ent.ItemID;
			LootItem item = LootItem.Items[itemID];

			item.GiveItem( ply, ent.Position );

			ent.Delete();
		}
	}
}
