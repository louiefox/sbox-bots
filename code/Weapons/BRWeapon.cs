using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using BattleRoyale;

partial class BaseBRWeapon : BaseWeapon
{
	public virtual string AmmoItemID => "ammo_pistol";
	public virtual int ClipSize => 16;
	public virtual float ReloadTime => 3.0f;
	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;

	[Net, Predicted]
	public int AmmoClip { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceReload { get; set; }

	[Net, Predicted]
	public bool IsReloading { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;
	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
	}

    public int AvailableAmmo()
    {
        if ( Owner is not BRPlayer player ) return 0;
        return player.GetInvItemCount( AmmoItemID );
    }

	public override void Reload()
	{
		if ( IsReloading )
			return;

		if ( AmmoClip >= ClipSize )
			return;

		TimeSinceReload = 0;

        if ( AvailableAmmo() <= 0 ) return;

        IsReloading = true;

		(Owner as AnimEntity).SetAnimBool( "b_reload", true );
    }

	public override void Simulate( Client owner ) 
	{
		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			base.Simulate( owner );
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}

		BRThirdPersonCamera camera = owner.Pawn.Camera as BRThirdPersonCamera;
		if ( Input.Down( InputButton.Attack2 ) )
		{
			camera.TargetFOV = 50f;
		} else
		{
			camera.TargetFOV = 70f;
		}
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;

		if ( Owner is BRPlayer player )
		{
            using( Prediction.Off() )
            {
                var ammo = player.TakeInvItems( AmmoItemID, ClipSize - AmmoClip );
                if ( ammo == 0 )
                    return;

                AmmoClip += ammo;
            }
		}
	}

	public override void AttackPrimary()
	{
        TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 5000 ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			//
			// We turn predictiuon off for this, so aany exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damage = DamageInfo.FromBullet( tr.EndPos, Owner.EyeRot.Forward * 100, 15 )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damage );
			}
		}
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		CrosshairPanel?.OnEvent( "fire" );
	}

	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
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

		var forward = Input.Rotation.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		forward = forward.Normal;

		var camTrace = Trace.Ray( pos, pos + forward * 5000 )
			.UseHitboxes()
			.Ignore( Owner )
			.Ignore( this )
			.Run();

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( Owner.EyePos, camTrace.EndPos, bulletSize ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			//
			// We turn predictiuon off for this, so any exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}

	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}

	public void DryFire()
	{
		Reload();
	}

	public override void CreateHudElements()
	{
		if ( Local.Hud == null ) return;

		CrosshairPanel = new Crosshair();
		CrosshairPanel.Parent = Local.Hud;
		CrosshairPanel.AddClass( ClassInfo.Name );
	}

	public bool IsUsable()
	{
		return AmmoClip > 0 || AvailableAmmo() > 0;
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );
	}
}
