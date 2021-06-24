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

        if ( LifeState != LifeState.Alive ) return;

        if ( Input.MouseWheel != 0 || Input.Pressed( InputButton.Slot1 ) || Input.Pressed( InputButton.Slot2 ) )
        {
            WeaponInventory.SelectNext();
        }

        if ( IsClient && Input.Pressed( InputButton.Use ) )
        {
            RequestLootPickup();
        }

        CheckArmourInsert();

        SimulateActiveChild( cl, ActiveChild );
    }

    public override void OnKilled()
    {
        base.OnKilled();

        (Game.Current as BRGame).CheckGameEnd( -1 );

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
            attacker.OnPlayerEliminated( this );
        }

        if( PlayerInfo.GetPlayerInfo( this ).State == PlayerGameState.Alive )
        {
            CreateDeadCamera();
            PlayerInfo.UpdateGameState( this, PlayerGameState.Dead );
            if( IsServer ) Delete();
        }

        // DEV TESTING
        //_ = StartRespawn();
    }

    // DEV TESTING
    /*private async System.Threading.Tasks.Task StartRespawn()
    {
        await Task.Delay( 1000 );
        Respawn();
    }*/

    private DamageInfo LastDamage;

    public override void TakeDamage( DamageInfo info )
    {
        if( BRGame.CurrentState == GameState.Starting || BRGame.CurrentState == GameState.Ended ) return;

        ResetRegen();
        CancelArmourInsert();

        LastDamage = info;

        // hack - hitbox 0 is head
        // we should be able to get this from somewhere
        if ( info.HitboxIndex == GetBoneIndex( "head" ) )
        {
            info.Damage *= 1.25f;
        }

        float oldArmour = Armour;
        if ( Armour > 0 )
        {
            Armour = Math.Max( Armour - info.Damage, 0 );

            info.Damage -= oldArmour - Armour;
        }

        base.TakeDamage( info );

        if ( info.Attacker is BRPlayer attacker && attacker != this )
        {
            attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Armour, oldArmour > 0 && Armour <= 0 );

            TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
        }
    }

    [ClientRpc]
    public void DidDamage( Vector3 pos, float amount, float armourLeft, bool brokeArmour )
    {
        if ( brokeArmour ) Sound.FromScreen( "armour_break" );

        if ( armourLeft > 0 ) Sound.FromScreen( "armour_hit" );
        else Sound.FromScreen( "dm.ui_attacker" );

        HitIndicator.Current?.OnHit( pos, amount, armourLeft, brokeArmour );
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
