
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace BattleRoyale.UI
{
	public class Scoreboard : Panel
	{
		private Panel Header;

		public Scoreboard()
		{
			StyleSheet.Load( "/ui/TabMenu/Scoreboard.scss" );

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
}
