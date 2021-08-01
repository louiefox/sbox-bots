using Sandbox;
using System;
using System.Linq;

public partial class DropShipCamera : Camera
{
	public DropShip TargetDropShip;

	public override void Update()
	{
		if ( TargetDropShip == null )
		{
			var dropShips = Entity.All.OfType<DropShip>();

			if ( dropShips.Count() > 0 )
				TargetDropShip = Entity.All.OfType<DropShip>().First();

			return;
		}

        Pos = TargetDropShip.Position + new Vector3( 0, 0, 300f ) + TargetDropShip.Rotation.Forward * 500;
		Rot = Rotation.From( TargetDropShip.Rotation.Pitch(), TargetDropShip.Rotation.Yaw() + 180f, TargetDropShip.Rotation.Roll() );

		FieldOfView = 70;

        Viewer = null;
	}
}
