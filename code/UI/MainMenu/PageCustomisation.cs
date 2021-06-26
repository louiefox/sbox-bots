
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale.UI.MainMenuPages
{
    public class PageCustomisation : Panel
    {
		private List<Page> Pages = new();
		private int ActivePage;
		private Panel NavigationPanel;
		private Dictionary<ClothingType, Panel> ClothingPages = new();

		private SceneCapture sceneCapture;
		private Angles CamAngles;
		private Panel ModelPanel;
		private Image ModelImage;

		public PageCustomisation()
        {
            StyleSheet.Load( "/ui/MainMenu/PageCustomisation.scss" );

			Panel leftSection = Add.Panel( "leftsection" );
			ModelPanel = leftSection.Add.Panel( "modelpanel" );
			CamAngles = new Angles( 25, 180, 0 );

			UpdateClothing();

			// Right Section
			Panel rightSection = Add.Panel( "rightsection" );

			NavigationPanel = rightSection.Add.Panel( "navigation" );
			Panel content = rightSection.Add.Panel( "content" );

			List<(ClothingType, string, string)> clothingPages = new() 
			{ 
				(ClothingType.Head, "Head", "sports_motorsports"),
				(ClothingType.Chest, "Chest", "checkroom"),
				(ClothingType.Hands, "Hands", "thumb_up"),
				(ClothingType.Legs, "Legs", "airline_seat_legroom_extra"),
				(ClothingType.Feet, "Feet", "do_not_step"),
			};

			foreach ( var info in clothingPages )
			{
				Panel page = content.Add.Panel( "page" );

				CreatePage( page, info.Item2, info.Item3 );
				ClothingPages.Add( info.Item1, page );
			}

			ClothingItemsUpdated();

			SetActivePage( 0 );
		}

		private void CreatePage( Panel panel, string title, string icon )
		{
			int index = Pages.Count;

			Panel pageButton = NavigationPanel.Add.Label( icon, "pagebutton" );
			pageButton.AddEventListener( "onclick", () => {
				SetActivePage( index );
			} );

			Pages.Add( new( panel, pageButton, title, icon ) );
		}

		private void SetActivePage( int index )
		{
			if ( ActivePage < Pages.Count )
			{
				Pages[ActivePage].Panel.RemoveClass( "active" );
				Pages[ActivePage].Button.RemoveClass( "active" );
			}

			Page pageInfo = Pages[index];
			pageInfo.Panel.AddClass( "active" );
			pageInfo.Button.AddClass( "active" );

			ActivePage = index;
		}

		[Event( "battleroyale.clothingitemsupdated" )]
		private void ClothingItemsUpdated()
		{
			foreach( var kv in ClothingPages )
			{
				kv.Value.DeleteChildren();

				var itemList = kv.Value.Add.Panel( "itemlist" );

				int rowItemsMax = 3;
				Panel currentRow = null;

				foreach ( var itemKv in ClothingItem.Items )
				{
					if ( itemKv.Value.Type != kv.Key ) continue;

					if ( currentRow == null || currentRow.ChildCount >= rowItemsMax )
						currentRow = itemList.Add.Panel( "itemrow" );

					CustomisationItem item = currentRow.AddChild<CustomisationItem>();
					item.SetItemInfo( itemKv.Value );
					item.Style.MarginRight = currentRow.ChildCount < rowItemsMax ? 10f : 0f;
				}

				if ( currentRow != null && currentRow.ChildCount < rowItemsMax )
					for ( int i = 0; i <= rowItemsMax - currentRow.ChildCount; i++ )
						currentRow.Add.Panel( "itementryfill" );
			}
		}		
		
		[Event( "battleroyale.updateclothing" )]
		private void UpdateClothing()
		{
			if ( Local.Pawn == null || Local.Pawn is not BRPlayer player ) return;

			RefreshScene( player.Clothing );
		}

		private void RefreshScene( Dictionary<ClothingType, string> clothing )
		{
			sceneCapture?.Delete();
			sceneCapture = null;

			using ( SceneWorld.SetCurrent( new SceneWorld() ) )
			{
				SceneObject.CreateModel( "models/citizen/citizen.vmdl", Transform.Zero );

				foreach ( var kv in clothing )
				{
					if ( !ClothingItem.Items.ContainsKey( kv.Value ) || ClothingItem.Items[kv.Value] is not ClothingItem item ) continue;
					SceneObject.CreateModel( item.Model, Transform.Zero );
				}

				SceneObject.CreateModel( "models/room.vmdl", Transform.Zero );

				Light.Point( Vector3.Up * 150.0f, 200.0f, Color.Red * 5000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 200, Color.White * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Backward * 100.0f, 200, Color.Magenta * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Right * 100.0f, 200, Color.Blue * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Left * 100.0f, 200, Color.Green * 15000.0f );

				sceneCapture = SceneCapture.Create( "menucustomisation", 512, 512 );

				sceneCapture.SetCamera( Vector3.Up * 30 + CamAngles.Direction * -150, CamAngles, 45 );
			}

			ModelImage?.Delete();
			ModelImage = ModelPanel.Add.Image( "scene:menucustomisation", "modelimage" );
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			sceneCapture?.Delete();
			sceneCapture = null;
		}

		public override void OnButtonEvent( ButtonEvent e )
		{
			if ( e.Button == "mouseleft" )
			{
				//SetMouseCapture( e.Pressed );
			}

			base.OnButtonEvent( e );
		}

		public override void Tick()
		{
			base.Tick();

			if ( HasMouseCapture )
			{
				CamAngles.pitch += Mouse.Delta.y;
				CamAngles.yaw -= Mouse.Delta.x;

				sceneCapture?.SetCamera( Vector3.Up * 30 + CamAngles.Direction * -150, CamAngles, 45 );
			}
		}

		public struct Page
		{
			public Panel Panel;
			public Panel Button;
			public string Title;
			public string Icon;

			public Page( Panel panel, Panel button, string title, string icon )
			{
				Panel = panel;
				Button = button;
				Title = title;
				Icon = icon;
			}
		}
	}

	public class CustomisationItem : Panel
	{
		private SceneCapture sceneCapture;

		public CustomisationItem()
		{
			SetClass( "itementry", true );
		}

		public void SetItemInfo( ClothingItem item )
		{
			Angles camAngles = new Angles( 0, 180, 0 );

			using ( SceneWorld.SetCurrent( new SceneWorld() ) )
			{
				SceneObject model = SceneObject.CreateModel( item.Model, Transform.Zero );
				SceneObject.CreateModel( "models/room.vmdl", Transform.Zero );

				Light.Point( Vector3.Up * 150.0f, 200.0f, Color.White * 5000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 200, Color.White * 15000.0f );

				sceneCapture = SceneCapture.Create( "clothing:" + item.Model, 512, 512 );

				sceneCapture.SetCamera( Vector3.Up * item.YDisplayOffset + camAngles.Direction * -50, camAngles, 60 );
			}

			Add.Image( "scene:clothing:" + item.Model, "itemimage" );

			Add.Label( item.Name, "itemname" );

			AddEventListener( "onclick", () => 
			{
				ConsoleSystem.Run( "request_equipclothing", item.ID );
			} );
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			sceneCapture?.Delete();
			sceneCapture = null;
		}
	}
}
