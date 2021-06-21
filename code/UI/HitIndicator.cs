using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

public partial class HitIndicator : Panel
{
	public static HitIndicator Current;

	public HitIndicator()
	{
		Current = this;
		StyleSheet.Load( "/ui/HitIndicator.scss" );

        //new HitPoint( this, 0, new Vector3(), 0, true );
    }

	public override void Tick()
	{
		base.Tick();
	}

	public void OnHit( Vector3 pos, float amount, float armourLeft, bool brokeArmour )
	{
		new HitPoint( this, amount, pos, armourLeft, brokeArmour );
	}

	public class HitPoint : Panel
	{
		public HitPoint( Panel parent, float amount, Vector3 pos, float armourLeft, bool brokeArmour )
		{
			Parent = parent;
			_ = Lifetime();

            if ( brokeArmour ) Add.Panel( "armour" ).AddClass( "broken" );
            else if ( armourLeft > 0 ) Add.Panel( "armour" );
		}

		async Task Lifetime()
		{
			await Task.Delay( 200 );
			Delete();
		}
	}
}


