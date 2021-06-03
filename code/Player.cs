using Sandbox;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using BattleRoyale;

partial class BRPlayer : Player
{
	public BRWeaponInventory WeaponInventory;

	public BRPlayer()
	{
		WeaponInventory = new BRWeaponInventory( this );
		MaxHealth = 100;
		MaxArmour = 150;
	}

	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		// Use WalkController for movement (you can make your own PlayerController for 100% control)
		Controller = new WalkController();
		/*(Controller as WalkController).WalkSpeed = 150f;
		(Controller as WalkController).DefaultSpeed = 100f;
		(Controller as WalkController).SprintSpeed = 220f;*/

		// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
		Animator = new StandardPlayerAnimator();

		Camera = new BRThirdPersonCamera();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		WeaponInventory.Add( 0, new Pistol(), true );
		WeaponInventory.Add( 1, new SMG() );

		GiveAmmo( AmmoType.Pistol, 100 );
		GiveAmmo( AmmoType.Buckshot, 8 );
		GiveAmmo( AmmoType.Crossbow, 4 );

		Health = MaxHealth;
		Armour = 100;

		base.Respawn();
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );


		if ( LifeState != LifeState.Alive )
			return;

		if ( Input.MouseWheel != 0 || Input.Pressed( InputButton.Slot1 ) || Input.Pressed( InputButton.Slot2 ) )
		{
			WeaponInventory.SelectNext();
		}

		if( IsClient && Input.Pressed( InputButton.Use ) )
		{
			RequestLootPickup();
		}

		TickPlayerUse();

		SimulateActiveChild( cl, ActiveChild );
	}

	public override void OnKilled()
	{
		base.OnKilled();

		BecomeRagdollOnClient( LastDamage.Force, GetHitboxBone( LastDamage.HitboxIndex ) );

		Controller = null;
		Camera = new SpectateRagdollCamera();

		EnableAllCollisions = false;
		EnableDrawing = false;
	}

	DamageInfo LastDamage;

	public override void TakeDamage( DamageInfo info )
	{
		ResetRegen();
		LastDamage = info;

		// hack - hitbox 0 is head
		// we should be able to get this from somewhere
		if ( info.HitboxIndex == 0 )
		{
			info.Damage *= 2.0f;
		}

		if ( Armour > 0 )
		{
			int oldArmour = Armour;
			Armour -= (int)info.Damage;

			info.Damage -= oldArmour - Armour;
		}

		base.TakeDamage( info );

		if ( info.Attacker is BRPlayer attacker && attacker != this )
		{
			// Note - sending this only to the attacker!
			attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, ((float)Health).LerpInverse( 100, 0 ) );

			TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
		}
	}

	[ClientRpc]
	public void DidDamage( Vector3 pos, float amount, float healthinv )
	{
		Sound.FromScreen( "dm.ui_attacker" )
			.SetPitch( 1 + healthinv * 1 );

		HitIndicator.Current?.OnHit( pos, amount );
	}

	[ClientRpc]
	public void TookDamage( Vector3 pos )
	{
		DamageIndicator.Current?.OnHit( pos );
	}
}
