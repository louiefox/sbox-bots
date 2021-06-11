
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
			StyleSheet.Load( "/ui/TabMenu/TabMenu.scss" );

			Inventory = AddChild<Inventory>();
			Scoreboard = AddChild<Scoreboard>();
		}

		public override void Tick()
		{
			base.Tick();

			SetClass( "open", Input.Down( InputButton.Score ) );
		}
	}
}
