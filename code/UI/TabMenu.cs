
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace BattleRoyale.UI
{
	public class TabMenu : Panel
	{
		public Inventory Inventory;
		public Scoreboard Scoreboard;

		public TabMenu()
		{
			StyleSheet.Load( "/ui/TabMenu.scss" );

			Inventory = AddChild<Inventory>();
			Scoreboard = AddChild<Scoreboard>();
		}

		public override void Tick()
		{
			base.Tick();

			SetClass( "open", Local.Client.Input.Pressed( InputButton.Score ) );
		}
	}

	public class Inventory : Panel
	{
		public Inventory()
		{

		}
	}	
	
	public class Scoreboard : Panel
	{
		private Panel Header;

		public Scoreboard()
		{
			AddHeader();
		}

		private void AddHeader()
		{
			Header = Add.Panel( "header" );
			Header.Add.Label( "player", "name" );
			Header.Add.Label( "kills", "kills" );
			Header.Add.Label( "deaths", "deaths" );
			Header.Add.Label( "ping", "ping" );
			Header.Add.Label( "fps", "fps" );
		}
	}

	public class ScoreboardEntry : Panel
	{
		public Label Fps;

		public ScoreboardEntry()
		{
			Fps = Add.Label( "", "fps" );
		}

		public void UpdateFrom( PlayerScore.Entry entry )
		{
			Fps.Text = entry.Get<int>( "fps", 0 ).ToString();
		}
	}

}
