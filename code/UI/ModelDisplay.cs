using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace BattleRoyale.UI
{
	public class ModelDisplay : Panel
	{
		//SceneCapture sceneCapture;
		Angles CamAngles;
		private Image ImgScene;

		public ModelDisplay()
		{
			Style.FlexWrap = Wrap.Wrap;
			Style.JustifyContent = Justify.Center; 
			Style.AlignItems = Align.Center;
			Style.AlignContent = Align.Center;
			StyleSheet.Parse( "image { width: 100%; height: 100%; justify-content: center; align-items: center; }" );
		}

		public void UpdateModel( string model, Angles angleOffset )
		{
			ImgScene?.Delete();

            if ( Model.Load( model ).IsError || true ) return;

			CamAngles = new Angles( 0, 0, 0 );
			CamAngles += angleOffset;

			using ( SceneWorld.SetCurrent( new SceneWorld() ) )
			{
				SceneObject.CreateModel( model, Transform.Zero );

				Light.Point( Vector3.Up * 150.0f, 200.0f, Color.White * 5000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Forward * 100.0f, 200, Color.White * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Backward * 100.0f, 200, Color.White * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Right * 100.0f, 200, Color.White * 15000.0f );
				Light.Point( Vector3.Up * 10.0f + Vector3.Left * 100.0f, 200, Color.White * 15000.0f );

				//sceneCapture = SceneCapture.Create( "modeldisplay_" + model, 512, 512 );

				//sceneCapture.SetCamera( CamAngles.Direction * -40, CamAngles, 45 );
			}

			ImgScene = Add.Image( "scene:" + "modeldisplay_" + model );
		}

		public void UpdateModel( string model )
		{
			UpdateModel( model, new Angles( 0, 0, 0 ) );
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			//sceneCapture?.Delete();
			//sceneCapture = null;
		}
	}
}
