using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

public class ClothingEntity : ModelEntity
{

}

partial class BRPlayer
{
	public Dictionary<ClothingType, string> CurrentClothing = new();
	private Dictionary<ClothingType, ModelEntity> ActiveModels = new();
	private float RainbowAngle;

	[Event.Tick]
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
	}

	public void Dress()
	{
		foreach( var kv in ActiveModels )
		{
			kv.Value?.Delete();
		}

		ActiveModels.Clear();

		CurrentClothing.Clear();
		CurrentClothing.Add( ClothingType.Torso, "models/citizen_clothes/jacket/suitjacket/suitjacket.vmdl" );
		CurrentClothing.Add( ClothingType.Legs, "models/citizen_clothes/trousers/smarttrousers/smarttrousers.vmdl" );
		CurrentClothing.Add( ClothingType.Feet, "models/citizen_clothes/shoes/smartshoes/smartshoes.vmdl" );

		foreach( var kv in CurrentClothing )
		{
			var ent = new ClothingEntity();
			ent.SetModel( kv.Value );
			ent.SetParent( this, true );
			ent.EnableShadowInFirstPerson = true;
			ent.EnableHideInFirstPerson = true;
			ent.RenderColor = Color.Random;

			var propInfo = ent.GetModel().GetPropData();
			if ( propInfo.ParentBodyGroupName != null ) SetBodyGroup( propInfo.ParentBodyGroupName, propInfo.ParentBodyGroupValue );

			ActiveModels.Add( kv.Key, ent );
		}

		/*if ( Rand.Int( 0, 3 ) != 1 )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/dress/dress.kneelength.vmdl",
				"models/citizen/clothes/trousers_tracksuit.vmdl",
				"models/citizen_clothes/shoes/shorts.cargo.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl"
			} );

			pants = new ClothingEntity();
			pants.SetModel( model );
			pants.SetParent( this, true );
			pants.EnableShadowInFirstPerson = true;
			pants.EnableHideInFirstPerson = true;

			if ( model.Contains( "dress" ) )
				jacket = pants;
		}

		if ( true )
		{
			jacket = new ClothingEntity();
			jacket.SetModel( "models/citizen_clothes/jacket/suitjacket/suitjacket.vmdl" );
			jacket.SetParent( this, true );
			jacket.EnableShadowInFirstPerson = true;
			jacket.EnableHideInFirstPerson = true;
		}

		if ( Rand.Int( 0, 3 ) != 1 )
		{
			shoes = new ClothingEntity();
			shoes.SetModel( "models/citizen_clothes/shoes/shoes.workboots.vmdl" );
			shoes.SetParent( this, true );
			shoes.EnableShadowInFirstPerson = true;
			shoes.EnableHideInFirstPerson = true;
		}

		if ( Rand.Int( 0, 3 ) != 1 )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/hat/hat_hardhat.vmdl",
				"models/citizen_clothes/hat/hat_woolly.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
				"models/citizen_clothes/hair/hair_malestyle02.vmdl",
				"models/citizen_clothes/hair/hair_femalebun.black.vmdl"
			} );

			hat = new ClothingEntity();
			hat.SetModel( model );
			hat.SetParent( this, true );
			hat.EnableShadowInFirstPerson = true;
			hat.EnableHideInFirstPerson = true;
		}*/
	}

	public enum ClothingType
	{
		Head,
		Torso,
		Hands,
		Legs,
		Feet
	}
}
