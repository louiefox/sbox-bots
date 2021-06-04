using Sandbox;
using System;

namespace BattleRoyale
{
	public class BRThirdPersonCamera : Camera
	{
		[ConVar.ClientData]
		public static bool thirdperson_collision { get; set; } = true;

		public float TargetFOV = 70;

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;
			var player = Local.Pawn as BRPlayer;
			var client = Local.Client;

			if ( pawn == null )
				return;

			Pos = pawn.Position;
			Vector3 targetPos;

			var center = pawn.Position + Vector3.Up * 64;

			if( (player.Controller as WalkController).Duck.IsActive )
			{
				center -= Vector3.Up * 25;
			}

			Pos = center;
			Rot = client.Input.Rotation;

			float distance = 130.0f * pawn.Scale;
			targetPos = Pos + client.Input.Rotation.Right * ((pawn.CollisionBounds.Maxs.x + 1) * pawn.Scale);
			targetPos += client.Input.Rotation.Forward * -distance;

			if ( thirdperson_collision )
			{
				var tr = Trace.Ray( Pos, targetPos )
					.Ignore( pawn )
					.Radius( 8 )
					.Run();

				Pos = tr.EndPos;
			}
			else
			{
				Pos = targetPos;
			}

			if( TargetFOV > FieldOfView )
			{
				FieldOfView = Math.Clamp( FieldOfView+1, 0, TargetFOV );
			} else
			{
				FieldOfView = Math.Clamp( FieldOfView-1, TargetFOV, FieldOfView );
			}

			Viewer = null;
		}
	}
}
