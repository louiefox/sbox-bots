using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

public class ClothingEntity : ModelEntity
{

}

partial class BRPlayer
{
	public Dictionary<ClothingType, string> Clothing = new();
	private Dictionary<ClothingType, ModelEntity> ActiveModels = new();

	public void Dress()
	{
		foreach( var kv in ActiveModels )
		{
			kv.Value?.Delete();
		}

		ActiveModels.Clear();

		foreach( var kv in Clothing )
		{
			ClothingItem item = ClothingItem.Items[kv.Value];

			var ent = new ClothingEntity();
			ent.SetModel( item.Model );
			ent.SetParent( this, true );
			ent.EnableShadowInFirstPerson = true;
			ent.EnableHideInFirstPerson = true;

			var propInfo = ent.GetModel().GetPropData();
			if ( propInfo.ParentBodyGroupName != null ) SetBodyGroup( propInfo.ParentBodyGroupName, propInfo.ParentBodyGroupValue );

			ActiveModels.Add( kv.Key, ent );
		}
	}

	public bool AddClothingItem( string id )
	{
		if ( !ClothingItem.Items.ContainsKey( id ) || ClothingItem.Items[id] is not ClothingItem item ) return false;

		Clothing[item.Type] = id;

		SendClothingAdded( To.Single( this ), id );

		return true;
	}

	[ClientRpc]
	public static void SendClothingAdded( string id )
	{
		if ( !ClothingItem.Items.ContainsKey( id ) || ClothingItem.Items[id] is not ClothingItem item || Local.Pawn is not BRPlayer player ) return;

		player.Clothing[item.Type] = id;
		Event.Run( "battleroyale.updateclothing" );
	}

	[ServerCmd( "request_equipclothing" )]
	public static void RequestEquipClothing( string id )
	{
		var owner = ConsoleSystem.Caller.Pawn as BRPlayer;
		if ( owner == null ) return;

		owner.AddClothingItem( id );
		owner.Dress();
	}

	//private float RainbowAngle;
	/*[Event.Tick]
	private void RainbowClothes()
	{
		if ( !ActiveModels.ContainsKey( ClothingType.Torso ) ) return;

		RainbowAngle += 3f;
		if ( RainbowAngle > 360 ) RainbowAngle = 0;

		float r;
		float g;
		float b;

		if ( RainbowAngle < 60 ) { r = 255; g = (int)Math.Round( RainbowAngle * 4.25 - 0.01 ); b = 0; }
		else
		if ( RainbowAngle < 120 ) { r = (int)Math.Round( (120 - RainbowAngle) * 4.25 - 0.01 ); g = 255; b = 0; }
		else
		if ( RainbowAngle < 180 ) { r = 0; g = 255; b = (int)Math.Round( (RainbowAngle - 120) * 4.25 - 0.01 ); }
		else
		if ( RainbowAngle < 240 ) { r = 0; g = (int)Math.Round( (240 - RainbowAngle) * 4.25 - 0.01 ); b = 255; }
		else
		if ( RainbowAngle < 300 ) { r = (int)Math.Round( (RainbowAngle - 240) * 4.25 - 0.01 ); g = 0; b = 255; }
		else { r = 255; g = 0; b = (int)Math.Round( (360 - RainbowAngle) * 4.25 - 0.01 ); }

		ActiveModels[ClothingType.Torso].RenderColor = new Color( r / 255, g / 255, b / 255 );
	}*/
}
