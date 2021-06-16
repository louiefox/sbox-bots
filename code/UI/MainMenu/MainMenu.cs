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

		public MainMenu()
		{
			StyleSheet.Load( "/ui/MainMenu/MainMenu.scss" );

            Panel menuPanel = Add.Panel( "menu" );
            Panel navigationPanel = menuPanel.Add.Panel( "navbar" );
            navigationPanel.Add.Panel( "menuicon" );

            Pages.Add( "stats", new( AddChild<PageStats>(), "Global Stats", "" ) );
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

            public Page( Panel panel, string title, string icon )
            {
                Panel = panel;
                Title = title;
                Icon = icon;
            }
        }
	}
}
