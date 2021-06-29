
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
		private Dictionary<string, CustomisationItem> ClothingItemPanels = new();

		private SceneWorld sceneWorld;
		private Angles CamAngles;
		private Panel ModelPanel;
		private Scene ModelImage;
		private List<SceneObject> SceneClothing = new();

		public PageCustomisation()
        {
            StyleSheet.Load( "/ui/MainMenu/PageCustomisation.scss" );

			Panel leftSection = Add.Panel( "leftsection" );
			ModelPanel = leftSection.Add.Panel( "modelpanel" );
			ModelPanel.AddEventListener( "OnMouseDown", () => ModelPanel.SetMouseCapture( true ) );
			ModelPanel.AddEventListener( "OnMouseUp", () => ModelPanel.SetMouseCapture( false ) );

			CamAngles = new Angles( 25, 180, 0 );
			CreateSceneWorld();
			ModelImage = ModelPanel.Add.Scene( sceneWorld, Vector3.Up * 30 + CamAngles.Direction * -150, CamAngles, 45, "modelimage" );

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
			ClothingItemPanels.Clear();

			foreach ( var kv in ClothingPages )
			{
				kv.Value.DeleteChildren();

				var itemList = kv.Value.Add.Panel( "itemlist" );

				int rowItemsMax = 3;
				Panel currentRow = null;

				foreach ( var itemKv in ClothingItem.Items )
				{
					ClothingItem item = itemKv.Value;
					if ( item.Type != kv.Key ) continue;

					if ( currentRow == null || currentRow.ChildCount >= rowItemsMax )
						currentRow = itemList.Add.Panel( "itemrow" );

					CustomisationItem itemPanel = currentRow.AddChild<CustomisationItem>();
					itemPanel.SetItemInfo( item );
					itemPanel.Style.MarginRight = currentRow.ChildCount < rowItemsMax ? 10f : 0f;

					ClothingItemPanels.Add( item.ID, itemPanel );
				}

				if ( currentRow != null && currentRow.ChildCount < rowItemsMax )
					for ( int i = 0; i <= rowItemsMax - currentRow.ChildCount; i++ )
						currentRow.Add.Panel( "itementryfill" );
			}

			UpdateActiveClothing();
		}		
		
		[Event( "battleroyale.updateclothing" )]
		private void UpdateClothing()
		{
			if ( Local.Pawn == null || Local.Pawn is not BRPlayer player ) return;

			RefreshScene( player.Clothing );
			UpdateActiveClothing();
		}

		private void UpdateActiveClothing()
		{
			Dictionary<ClothingType, string> clothing = new();
			if ( Local.Pawn != null && Local.Pawn is BRPlayer player ) clothing = player.Clothing;

			foreach ( var kv in ClothingItemPanels )
			{
				if ( !ClothingItem.Items.ContainsKey( kv.Key ) ) return;
				ClothingItem item = ClothingItem.Items[kv.Key];

				kv.Value.SetActive( clothing.ContainsKey( item.Type ) && clothing[item.Type] == item.ID );
			}
		}

		private void CreateSceneWorld()
		{
			sceneWorld = new SceneWorld();

			using ( SceneWorld.SetCurrent( sceneWorld ) )
			{
				SceneObject.CreateModel( "models/citizen/citizen.vmdl", Transform.Zero );
				SceneObject.CreateModel( "models/room.vmdl", Transform.Zero );

				Light.Point( Vector3.Up * 150.0f, 200.0f, Color.Red * 5000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 200, Color.White * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Backward * 100.0f, 200, Color.Magenta * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Right * 100.0f, 200, Color.Blue * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Left * 100.0f, 200, Color.Green * 15000.0f );
			}
		}

		private void RefreshScene( Dictionary<ClothingType, string> clothing )
		{
			foreach( SceneObject obj in SceneClothing )
			{
				obj?.Delete();
			}

			SceneClothing.Clear();

			using ( SceneWorld.SetCurrent( sceneWorld ) )
			{
				foreach ( var kv in clothing )
				{
					if ( !ClothingItem.Items.ContainsKey( kv.Value ) || ClothingItem.Items[kv.Value] is not ClothingItem item ) continue;
					SceneClothing.Add( SceneObject.CreateModel( item.Model, Transform.Zero ) );
				}
			}
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			sceneWorld?.Delete();
		}

		public override void Tick()
		{
			base.Tick();

			if ( ModelPanel.HasMouseCapture )
			{
				CamAngles.pitch += Mouse.Delta.y;
				CamAngles.yaw -= Mouse.Delta.x;

				if ( ModelImage == null ) return;
				ModelImage.Position = Vector3.Up * 30 + CamAngles.Direction * -150;
				ModelImage.Angles = CamAngles;
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
		private SceneWorld sceneWorld;
		private Panel ActiveIcon;

		public CustomisationItem()
		{
			SetClass( "itementry", true );
		}

		public void SetItemInfo( ClothingItem item )
		{
			sceneWorld = new SceneWorld();
			Angles camAngles = new Angles( 0, 180, 0 );

			using ( SceneWorld.SetCurrent( sceneWorld ) )
			{
				SceneObject.CreateModel( item.Model, Transform.Zero );

				Light.Point( Vector3.Up * 150.0f + camAngles.Direction * -50, 2000.0f, Color.White * 50000.0f );
			}

			Add.Scene( sceneWorld, Vector3.Up * item.YDisplayOffset + camAngles.Direction * -50, camAngles, 60, "itemimage" );

			Add.Label( item.Name, "itemname" );

			AddEventListener( "onclick", () => 
			{
				ConsoleSystem.Run( "request_equipclothing", item.ID );
			} );
		}

		public void SetActive( bool active )
		{
			SetClass( "active", active );

			if ( !active )
			{
				ActiveIcon?.Delete();
				ActiveIcon = null;
				return;
			}

			if ( active && ActiveIcon != null ) return;

			ActiveIcon = Add.Label( "check_circle", "activemark" );
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			sceneWorld?.Delete();
		}
	}
}
