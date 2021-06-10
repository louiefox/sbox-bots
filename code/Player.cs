using Sandbox;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using BattleRoyale;

partial class BRPlayer : Player
{
    public BRWeaponInventory WeaponInventory;
    public BRInventory ItemInventory;

    public BRPlayer()
    {
        WeaponInventory = new BRWeaponInventory( this );
        ItemInventory = new BRInventory( this );

        MaxArmour = 150;
    }

    public override void Respawn()
    {
        SetModel( "models/citizen/citizen.vmdl" );

        Controller = new WalkController();
        Animator = new StandardPlayerAnimator();
        Camera = new BRThirdPersonCamera();
        
        EnableAllCollisions = true;
        EnableDrawing = true;

        Health = 100f;
        Armour = 100;

        WeaponInventory.Add( 0, new Pistol() );

        using( Prediction.Off() )
        {
            ItemInventory.Add( new BRInventoryItem( "armour_plate", 1 ) );
            ItemInventory.Add( new BRInventoryItem( "ammo_pistol", 30 ) );
            ItemInventory.Add( new BRInventoryItem( "ammo_shotgun", 10 ) );
            ItemInventory.Add( new BRInventoryItem( "ammo_crossbow", 4 ) );
            ItemInventory.Add( new BRInventoryItem( "ammo_rifle", 20 ) );
        }

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

        if ( IsClient && Input.Pressed( InputButton.Use ) )
        {
            RequestLootPickup();
        }

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

        if( BRGame.CurrentState == GameState.Active )
        {
            foreach ( var data in WeaponInventory.Weapons )
            {
                WeaponInventory.Drop( data.Key );
            }

            foreach ( var data in ItemInventory.Slots )
            {
                ItemInventory.Drop( data.Key );
            }
        } else
        {
            foreach ( var data in WeaponInventory.Weapons )
            {
                WeaponInventory.Remove( data.Key, true );
            }

            foreach ( var data in ItemInventory.Slots )
            {
                ItemInventory.Remove( data.Key );
            }
        }

        if ( LastAttacker is BRPlayer attacker )
        {
            PlayerInfo.UpdateKills( attacker, 1 );
        }

        if( PlayerInfo.GetPlayerInfo( this ).State == PlayerGameState.Alive )
        {
            CreateDeadCamera();
            PlayerInfo.UpdateGameState( this, PlayerGameState.Dead );
            if( IsServer ) Delete();
        }
    }

    DamageInfo LastDamage;

    public override void TakeDamage( DamageInfo info )
    {
        if( BRGame.CurrentState == GameState.Starting || BRGame.CurrentState == GameState.Ended ) return;

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
            float oldArmour = Armour;
            Armour = Math.Max( Armour - info.Damage, 0 );

            info.Damage -= oldArmour - Armour;
        }

        base.TakeDamage( info );

        if ( info.Attacker is BRPlayer attacker && attacker != this )
        {
            // Note - sending this only to the attacker!
            attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Health.LerpInverse( 100, 0 ) );

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

    public void CreateDeadCamera()
    {
        Client client = GetClientOwner();
        client.Camera = new BRSpectateCamera();
    }
}
