using Sandbox;
using System;
using System.Linq;

[Library( "ent_dropship" )]
public partial class DropShip : Prop
{
	private float TravelTime = 10f;
	private TimeSince StartTime;
	private bool Travelling;
	private Vector3 StartPos;
	private Vector3 EndPos;
	public DropShipCamera DropCamera;

	public override void Spawn()
	{
		base.Spawn();

		DropCamera = new();

		SetModel( "models/bots/helicopter/helicopter.vmdl_c" );

        GlowState = GlowStates.GlowStateOn;
		GlowDistanceStart = 0;
		GlowDistanceEnd = 1000;
		GlowColor = new Color( 0.0f, 1.0f, 0.0f, 1.0f );
		GlowActive = true;
	}

	public void StartMoving()
	{
		DropPathStart pathStart = All.OfType<DropPathStart>().First();
		DropPathEnd pathEnd = All.OfType<DropPathEnd>().First();

		StartPos = pathStart.Position;
		EndPos = pathEnd.Position;
		Position = StartPos;

		float xDist = Math.Abs( StartPos.x - EndPos.x );
		float yDist = Math.Abs( StartPos.y - EndPos.y );

		double angle = Math.Atan( yDist / xDist ) * 180 / Math.PI;

		Rotation = Rotation.From( Rotation.Pitch(), Rotation.Yaw() + (float)angle, Rotation.Roll() );

		StartTime = 0;
		Travelling = true;
	}

	protected override void OnDestroy()
	{
		foreach ( Client client in Client.All )
		{
			client.Camera = client.Pawn.Camera;
		}

		DropCamera = null;
	}

	[Event( "server.tick" )]
	private void TravelMove()
	{
		if ( !Travelling ) return;

		float progress = Math.Clamp( StartTime / TravelTime, 0, 1 );
		Position = new Vector3( progress * (EndPos.x - StartPos.x), progress * (EndPos.y - StartPos.y), Position.z );

		if ( StartTime >= TravelTime ) Delete();
	}
}
