using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class ElimIndicator : Panel
{
	public static ElimIndicator Current;
	private ElimPoint CurrentPoint;

	public ElimIndicator()
	{
		Current = this;
		StyleSheet.Load( "/ui/ElimIndicator.scss" );

        //new ElimPoint( this, "Brickwall", new() { EliminationTag.Types.Headshot, EliminationTag.Types.DoubleKill } );
    }

	public override void Tick()
	{
		base.Tick();
	}

	public void OnElimination( string name, List<EliminationTag.Types> tags )
	{
        CurrentPoint?.Delete( true );
        CurrentPoint = new ElimPoint( this, name, tags );
	}

	public class ElimPoint : Panel
	{
		public ElimPoint( Panel parent, string name, List<EliminationTag.Types> tags )
		{
			Parent = parent;

            Panel title = Add.Panel( "title" );
            title.Add.Label( "Eliminated " );
            title.Add.Label( name, "name" );

            List<Label> tagLabels = new();
            foreach ( EliminationTag.Types type in tags )
            {
                EliminationTag tagInfo = EliminationTag.Tags[type];
                tagLabels.Add( Add.Label( tagInfo.Title, "tag" ) );
            }

            _ = CreateTags( tagLabels );
		}

		async Task CreateTags( List<Label> tagLabels )
		{
            foreach ( Label label in tagLabels )
            {
                await Task.Delay( 200 );
                label.AddClass( "active" );
            }

            _ = Lifetime();
        }

        async Task Lifetime()
        {
            await Task.Delay( 1500 );
            Delete();
        }
    }
}
