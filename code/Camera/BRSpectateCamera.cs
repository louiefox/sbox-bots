using Sandbox;
using System;

namespace BattleRoyale
{
	public class BRSpectateCamera : Camera
	{
        public Player CurrentTarget;

        private void FindTarget()
        {
            CurrentTarget = null;
            foreach ( var kv in PlayerInfo.Players )
            {
                if ( kv.Value.State != PlayerGameState.Alive || kv.Value.Client.Pawn is not BRPlayer ply ) continue;
                CurrentTarget = ply;
                break;
            }
        }

		public override void Update()
		{
			if ( CurrentTarget == null || !CurrentTarget.IsValid() )
            {
                FindTarget();
                return;
            }

            if ( PlayerInfo.GetPlayerInfo( CurrentTarget ) is not PlayerInfo playerInfo || playerInfo.State != PlayerGameState.Alive )
            {
                FindTarget();
            }

            if ( CurrentTarget == null ) return;
            Rotation targetRot = CurrentTarget.EyeRot;

            Vector3 targetPos;
            float distance = 130.0f * CurrentTarget.Scale;
            targetPos = CurrentTarget.Position + Vector3.Up * 64 + targetRot.Right * ((CurrentTarget.CollisionBounds.Maxs.X + 1) * CurrentTarget.Scale);
            targetPos += targetRot.Forward * -distance;
            
            Pos = targetPos;
			Rot = targetRot;
            FieldOfView = 70;

            Viewer = null;
		}
	}
}
