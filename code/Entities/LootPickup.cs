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
	private float MinLootDistance = 30f;

	public void SetItem(string itemID)
	{
		ItemID = itemID;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		CreateClientModel();
	}	
	
	public void SetPosition( Vector3 pos )
	{
		Position = DropToFloor( pos );
		if ( CheckPosition( Position ) ) return;

		bool positionClear = false;
		int checkCount = 0;
		while ( !positionClear )
		{
			checkCount++;

			int layerSize = 3;
			while ( checkCount >= (layerSize * layerSize) )
			{
				layerSize += 2;
			}


			if ( layerSize > 9 )
			{
				Log.Info( "BattleRoyale ERROR: Cannot find position for loot pickup." );
				break;
			}

			int totalLayerCount = (layerSize * 4) - 4;
			int currentLayerCount = checkCount + 1 - ((layerSize - 2) * (layerSize - 2));
			int currentSideCount = currentLayerCount;

			int side = 1;
			if ( currentLayerCount > totalLayerCount - layerSize )
			{
				side = 4;
				currentSideCount -= ((layerSize - 2) * 2) + layerSize;
			}
			else if ( currentLayerCount > (layerSize - 2) + layerSize )
			{
				side = 3;
				currentSideCount -= (layerSize - 2) + layerSize;
			}
			else if ( currentLayerCount > (layerSize - 2) )
			{
				side = 2;
				currentSideCount -= (layerSize - 2);
			}

			int xDiff;
			int yDiff;
			int halfWay = (int)Math.Ceiling( (layerSize - 2) / 2f );

			if ( side == 1 || side == 3 )
			{
				xDiff = currentSideCount <= halfWay ? currentSideCount - 1 : -(currentSideCount - halfWay);
				yDiff = side == 1 ? halfWay : -halfWay;
			} else
			{
				xDiff = side == 2 ? halfWay : -halfWay;

				int fullHalfWay = (int)Math.Ceiling( layerSize / 2f );
				yDiff = currentSideCount <= fullHalfWay ? currentSideCount - 1 : -(currentSideCount - fullHalfWay);
			}

			Vector3 checkPos = Position + new Vector3( MinLootDistance * xDiff, MinLootDistance * yDiff, 0 );

			if ( !CheckPosition( checkPos ) || !CheckForWall( Position, checkPos ) ) continue;

			Position = DropToFloor( checkPos );
			positionClear = true;
		}
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
	
	public bool CheckForWall( Vector3 fromPos, Vector3 pos )
	{
		var tr = Trace.Ray( fromPos, pos )
			.WorldOnly()
			.Run();

		return tr.EndPos == pos;
	}	
	
	public Vector3 DropToFloor( Vector3 pos )
	{
		var tr = Trace.Ray( pos + new Vector3( 0, 0, 50f ), pos - new Vector3( 0, 0, 1000f ) )
			.WorldOnly()
			.Run();

		return tr.EndPos + new Vector3( 0, 0, 20f );
	}

	[ClientRpc]
	public void CreateClientModel()
	{
		LootItem item = LootItem.Items[ItemID];

		switch (item.Rarity)
		{
			case ItemRarity.Common:
				RarityColor = new Color32( 100, 100, 100 );
				break;			
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
		ClientModel.GlowColor = new Color( 1.0f, 1.0f, 1.0f );
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
		ClientModel.GlowColor = new Color( 1.0f, 1.0f, 1.0f );
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

		if( item.Type == ItemType.Weapon )
		{
			Delete();
		}

		if( item.GiveItem( ply, Position ) && this.IsValid() )
		{
			Delete();
		}
	}
}
