using Sandbox;
using System;
using System.Collections.Generic;

[Library( "ent_lootpickup" )]
public partial class LootPickup : FloorUsable
{
	[Net]
	public string ItemID { get; set; }

	public Prop ClientModel;
	private float ClientZRot = 0f;
	private bool ClientBobIncreasing = true;
	private float ClientZBob = 0f;

	private Color32 RarityColor;
	private float MinLootDistance = 50f;

	public void SetItem(string itemID)
	{
		ItemID = itemID;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		if( !CheckPosition( Position ) )
		{
			bool positionClear = false;
			int checkCount = 0;
			while( !positionClear )
			{
				checkCount++;

				int layer = (int)Math.Floor( checkCount / 4f ) + 1;
				if( layer > 10 )
				{
					Log.Info( "BattleRoyale ERROR: Cannot find position for loot pickup." );
					break;
				}

				int side = checkCount - ((layer-1) * 4) + 1;
				Vector3 checkPos = Position;
				switch( side )
				{
					case 1:
						checkPos += new Vector3( MinLootDistance * layer, 0, 0 );
						break;					
					case 2:
						checkPos -= new Vector3( MinLootDistance * layer, 0, 0 );
						break;					
					case 3:
						checkPos += new Vector3( 0, MinLootDistance * layer, 0 );
						break;
					case 4:
						checkPos -= new Vector3( 0, MinLootDistance * layer, 0 );
						break;
				}

				if( CheckPosition( checkPos ) )
				{
					Position = checkPos;
					positionClear = true;
				}
			}
		}

		CreateClientModel();
	}

	public bool CheckPosition( Vector3 pos )
	{
		foreach( var data in IndexEnts )
		{
			if( data.Value is LootPickup lootEnt && lootEnt != this && pos.Distance( lootEnt.Position ) < MinLootDistance )
			{
				return false;
			}
		}

		return true;
	}

	[ClientRpc]
	public void CreateClientModel()
	{
		LootItem item = LootItem.Items[ItemID];

		switch (item.Rarity)
		{
			case ItemRarity.Uncommon:
				RarityColor = new Color32( 75, 235, 61 );
				break;
			case ItemRarity.Rare:
				RarityColor = new Color32( 67, 61, 235 );
				break;			
			case ItemRarity.Epic:
				RarityColor = new Color32( 154, 61, 235 );
				break;			
			case ItemRarity.Legendary:
				RarityColor = new Color32( 235, 177, 61 );
				break;
			default:
				RarityColor = new Color32( 176, 176, 176 );
				break;
		}

		ClientModel = new Prop();
		ClientModel.Parent = this;
		ClientModel.Position = Position;
		ClientModel.SetModel( item.Model );

		ClientModel.GlowState = GlowStates.GlowStateOn;
		ClientModel.GlowDistanceStart = 0;
		ClientModel.GlowDistanceEnd = 1000;
		ClientModel.GlowColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		ClientModel.GlowActive = true;
	}
	
	[Event( "client.tick" )]
	public void UpdateClientModel()
	{
		if ( ClientModel == null || !ClientModel.IsValid() ) return;

		
		if( ClientBobIncreasing )
		{
			ClientZBob += .1f;
			ClientBobIncreasing = ClientZBob >= 10f ? false : true;
		} else
		{
			ClientZBob -= .1f;
			ClientBobIncreasing = ClientZBob <= 0f ? true : false;
		}

		ClientModel.Position = Position + new Vector3( 0, 0, ClientZBob );

		ClientZRot = ClientZRot >= 360 ? 0 : ClientZRot + .5f;
		ClientModel.Rotation = Rotation.From( 0f, ClientZRot, 0f );
	}

	public override void EnableGlow()
	{
		ClientModel.GlowColor = RarityColor.ToColor();
	}	
	
	public override void DisableGlow()
	{
		ClientModel.GlowColor = new Color( 1.0f, 1.0f, 1.0f, .1f );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		ClientModel?.Delete();
	}

	public override void Use( Player ply )
	{
		if ( ply.Position.Distance( Position ) > 200 ) return;

		LootItem item = LootItem.Items[ItemID];

		if( item.GiveItem( ply, Position ) )
		{
			Delete();
		}
	}
}
