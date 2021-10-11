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

		Rot = Rotation.From( Input.Rotation.Pitch(), Input.Rotation.Yaw(), TargetDropShip.Rotation.Roll() );
		Pos = TargetDropShip.Position + (Rot.Up * 200) + (Rot.Forward * -800);

		FieldOfView = 70;

        Viewer = null;
	}
}
