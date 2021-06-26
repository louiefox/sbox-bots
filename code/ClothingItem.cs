using Sandbox;
using System;
using System.Collections.Generic;
using BattleRoyale;

public class ClothingItem
{
    public static Dictionary<string, ClothingItem> Items = new();

    public string ID;
    public string Name;
    public string Model;
    public ClothingType Type;

	public float YDisplayOffset = 20f;

    public ClothingItem( string id, string name, string model, ClothingType type )
    {
		ID = id;
		Name = name;
        Model = model;
		Type = type;

		switch(type)
		{
			case ClothingType.Head:
				YDisplayOffset = 70f;
				break;			
			case ClothingType.Chest:
				YDisplayOffset = 45f;
				break;			
			case ClothingType.Legs:
				YDisplayOffset = 30f;
				break;			
			case ClothingType.Hands:
				YDisplayOffset = 40f;
				break;
		}

		Items.Add( id, this );
	}

	public static void LoadClothing()
	{
		Items.Clear();

		new ClothingItem( "hat_hard", "Hardhat", "models/citizen_clothes/hat/hat_hardhat.vmdl", ClothingType.Head );
		new ClothingItem( "hat_woolly", "Woolly Hat", "models/citizen_clothes/hat/hat_woolly.vmdl", ClothingType.Head );
		new ClothingItem( "hat_woolly_bobble", "Woolly Bobble Hat", "models/citizen_clothes/hat/hat_woollybobble.vmdl", ClothingType.Head );
		new ClothingItem( "hat_security_strap", "Strap Security Helmet", "models/citizen_clothes/hat/hat_securityhelmet.vmdl", ClothingType.Head );
		new ClothingItem( "hat_security", "Security Helmet", "models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl", ClothingType.Head );
		new ClothingItem( "hat_beret_red", "Red Beret", "models/citizen_clothes/hat/hat_beret.red.vmdl", ClothingType.Head );
		new ClothingItem( "hat_beret_black", "Black Beret", "models/citizen_clothes/hat/hat_beret.black.vmdl", ClothingType.Head );
		new ClothingItem( "hat_tophat", "Top Hat", "models/citizen_clothes/hat/hat.tophat.vmdl", ClothingType.Head );
		new ClothingItem( "hat_cap", "Cap", "models/citizen_clothes/hat/hat_cap.vmdl", ClothingType.Head );
		new ClothingItem( "hat_leather_cap", "Leather Cap Badge", "models/citizen_clothes/hat/hat_leathercap.vmdl", ClothingType.Head );
		new ClothingItem( "hat_leather_cap_nobadge", "Leather Cap", "models/citizen_clothes/hat/hat_leathercapnobadge.vmdl", ClothingType.Head );
		new ClothingItem( "hat_service", "Service Hat", "models/citizen_clothes/hat/hat_service.vmdl", ClothingType.Head );
		new ClothingItem( "hat_police", "Police Hat", "models/citizen_clothes/hat/hat_uniform.police.vmdl", ClothingType.Head );
		new ClothingItem( "hair_malestyle02", "Male Hair", "models/citizen_clothes/hair/hair_malestyle02.vmdl", ClothingType.Head );
		new ClothingItem( "hair_femalebun", "Female Bun", "models/citizen_clothes/hair/hair_femalebun.black.vmdl", ClothingType.Head );

		new ClothingItem( "jacket_suit", "Suit Jacket", "models/citizen_clothes/jacket/suitjacket/suitjacket.vmdl", ClothingType.Chest );
		new ClothingItem( "jacket_labcoat", "Labcoat", "models/citizen_clothes/jacket/labcoat.vmdl", ClothingType.Chest );
		new ClothingItem( "jacket_red", "Red Jacket", "models/citizen_clothes/jacket/jacket.red.vmdl", ClothingType.Chest );
		new ClothingItem( "jacket_tuxedo", "Tuxedo", "models/citizen_clothes/jacket/jacket.tuxedo.vmdl", ClothingType.Chest );
		new ClothingItem( "jacket_heavy", "Heavy Jacket", "models/citizen_clothes/jacket/jacket_heavy.vmdl", ClothingType.Chest );

		new ClothingItem( "gloves_work", "Work Gloves", "models/citizen_clothes/gloves/gloves_workgloves.vmdl", ClothingType.Hands );

		new ClothingItem( "trousers_suit", "Suit Trousers", "models/citizen_clothes/trousers/smarttrousers/smarttrousers.vmdl", ClothingType.Legs );
		new ClothingItem( "cargo_shorts", "Cargo Shorts", "models/citizen_clothes/shoes/shorts.cargo.vmdl", ClothingType.Legs );
		new ClothingItem( "trousers_tracksuit", "Tracksuit Trousers", "models/citizen/clothes/trousers_tracksuit.vmdl", ClothingType.Legs );
		new ClothingItem( "trousers_jeans", "Jeans Trousers", "models/citizen_clothes/trousers/trousers.jeans.vmdl", ClothingType.Legs );
		new ClothingItem( "trousers_lab", "Lab Trousers", "models/citizen_clothes/trousers/trousers.lab.vmdl", ClothingType.Legs );
		new ClothingItem( "trousers_police", "Police Trousers", "models/citizen_clothes/trousers/trousers.police.vmdl", ClothingType.Legs );
		new ClothingItem( "trousers_smart", "Smart Trousers", "models/citizen_clothes/trousers/trousers.smart.vmdl", ClothingType.Legs );
		new ClothingItem( "trousers_smarttan", "Smart Tan Trousers", "models/citizen_clothes/trousers/trousers.smarttan.vmdl", ClothingType.Legs );

		new ClothingItem( "shoes_smart", "Smart Shoes", "models/citizen_clothes/shoes/smartshoes/smartshoes.vmdl", ClothingType.Feet );
		new ClothingItem( "shoes_trainers", "Trainers", "models/citizen_clothes/shoes/trainers.vmdl", ClothingType.Feet );
		new ClothingItem( "shoes_workboots", "Workboots", "models/citizen_clothes/shoes/shoes.workboots.vmdl", ClothingType.Feet );

		Event.Run( "battleroyale.clothingitemsupdated" );
	}
}

public enum ClothingType
{
	Head,
	Chest,
	Hands,
	Legs,
	Feet
}
