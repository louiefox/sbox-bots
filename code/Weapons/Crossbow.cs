using Sandbox;

[Library( "dm_crossbow", Title = "Crossbow" )]
partial class Crossbow : BaseBRWeapon
{ 
	public override float PrimaryRate => 1;
	public override int Bucket => 3;
	public override string AmmoItemID => "ammo_crossbow";
    public override int ClipSize => 1;

    [Net]
	public bool Zoomed { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		AmmoClip = ClipSize;
		SetModel( "weapons/rust_crossbow/rust_crossbow.vmdl" );
	}

	public override void AttackPrimary()
	{
		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		ShootEffects();
        PlaySound( "rust_crossbow.shoot" );

        if ( IsServer )
		using ( Prediction.Off() )
		{
            Vector3 pos = Owner.Position;
            Vector3 targetPos;

            var center = pos + Vector3.Up * 64;

            if ( ((Owner as Player).Controller as WalkController).Duck.IsActive )
            {
                center -= Vector3.Up * 25;
            }

            pos = center;

            float distance = 130.0f * Owner.Scale;
            targetPos = pos + Input.Rotation.Right * (((Owner as Player).CollisionBounds.Maxs.x + 1) * Owner.Scale);
            targetPos += Input.Rotation.Forward * -distance;

            pos = Trace.Ray( pos, targetPos )
                .Ignore( Owner )
                .Radius( 8 )
                .Run().EndPos;

            var camTrace = Trace.Ray( pos, pos + Input.Rotation.Forward * 5000 )
                .UseHitboxes()
                .Ignore( Owner )
                .Ignore( this )
                .Run();

            var bolt = new CrossbowBolt();
			bolt.Position = Owner.EyePos;
            bolt.Rotation = Owner.EyeRot;
			bolt.Owner = Owner;
			bolt.Velocity = camTrace.EndPos;
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Zoomed = Input.Down( InputButton.Attack2 );
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( Zoomed )
		{
			camSetup.FieldOfView = 20;
		}
	}

	public override void BuildInput( InputBuilder owner ) 
	{
		if ( Zoomed )
		{
			owner.ViewAngles = Angles.Lerp( owner.OriginalViewAngles, owner.ViewAngles, 0.2f );
		}
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin( 0.5f, 4.0f, 1.0f, 0.5f );
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
        Event.Run( "battleroyale.weaponfired" );
    }
}
