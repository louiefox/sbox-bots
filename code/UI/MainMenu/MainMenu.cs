using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using BattleRoyale.UI.MainMenuPages;

namespace BattleRoyale.UI
{
	public class MainMenu : Panel
	{
        private Dictionary<string, Page> Pages = new();
        private Dictionary<string, Panel> PageButtons = new();
        private string ActivePage = "";
        private Label PageTitle;
        private Label PageDescription;

		public MainMenu()
		{
			StyleSheet.Load( "/ui/MainMenu/MainMenu.scss" );

            Panel menuPanel = Add.Panel( "menu" );
            Panel navigationPanel = menuPanel.Add.Panel( "navbar" );

            Panel menuIcon = navigationPanel.Add.Icon( "menu" );
            menuIcon.AddClass( "menuicon" );

            Panel mainArea = menuPanel.Add.Panel( "mainarea" );

            Panel topBar = mainArea.Add.Panel( "topbar" );

            Panel infoContainer = topBar.Add.Panel( "pageinfocontainer" );
            PageTitle = infoContainer.Add.Label( "", "pagelabel" );
            PageDescription = infoContainer.Add.Label( "", "pagedescription" );

            Panel pageArea = mainArea.Add.Panel( "pagearea" );

            Pages.Add( "stats", new( pageArea.AddChild<PageStats>( "page" ), "Global Stats", "leaderboard", "View the global stats of players" ) );
            Pages.Add( "customisation", new( pageArea.AddChild<PageCustomisation>( "page" ), "Customisation", "person", "Customise your character with cosmetics" ) );
            Pages.Add( "settings", new( pageArea.AddChild<PageSettings>( "page" ), "Settings", "tune", "Change client settings" ) );

            foreach( var kv in Pages )
            {
                Page pageInfo = kv.Value;

                IconPanel pageButton = navigationPanel.Add.Icon( pageInfo.Icon );
                pageButton.AddClass( "pagebutton" );
                pageButton.AddEvent( "onclick", () => {
                    SetActivePage( kv.Key );
                } );

                PageButtons.Add( kv.Key, pageButton );

                if ( !Pages.ContainsKey( ActivePage ) )
                {
                    SetActivePage( kv.Key );
                }
            }
        }

        private void SetActivePage( string key )
        {
            if ( Pages.ContainsKey( ActivePage ) )
            {
                Pages[ActivePage].Panel.RemoveClass( "active" );
                PageButtons[ActivePage].RemoveClass( "active" );
            }

            Page pageInfo = Pages[key];
            pageInfo.Panel.AddClass( "active" );
            PageButtons[key].AddClass( "active" );

            PageTitle.Text = pageInfo.Title.ToUpper();
            PageDescription.Text = pageInfo.Description;

            ActivePage = key;
        }

		public override void Tick()
		{
			base.Tick();

			SetClass( "open", Input.Down( InputButton.Menu ) || true );
		}

        public struct Page
        {
            public Panel Panel;
            public string Title;
            public string Icon;
            public string Description;

            public Page( Panel panel, string title, string icon, string description )
            {
                Panel = panel;
                Title = title;
                Icon = icon;
                Description = description;
            }
        }
	}
}
