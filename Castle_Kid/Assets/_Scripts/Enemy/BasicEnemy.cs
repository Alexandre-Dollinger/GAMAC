using UnityEngine;
using System;
using _Scripts.Player.Movement;
using Unity.Netcode;
using _Scripts.GameManager;
using _Scripts.Enemy;
using _Scripts.Health;

public abstract class BasicEnemy : NetworkBehaviour
{
    //Base class that all enemies will inherit from
    #region Player variable
    protected GameObject closestPlayer;
    protected PlayerMovement closestPlayerMovement;
    protected float closestPlayerCooldown; // The one which is updated
    protected float closestPlayerTime; // Max time
    #endregion

    protected Rigidbody2D enemyRb;
    protected EnemyType enemyType;

    #region Booleans
    protected bool isFacingRight;
    protected bool willFall;
    protected bool isGrounded;
    protected bool isChasing;
    #endregion


    //Combat Stats
    protected int AttackPower {get; set;} //How much damage the enemy deals per attack
    
    //Drops
    protected int MoneyDrop {get;}
    protected bool IsGuardian {get;} //Rare enemy

    #region Movement Stats
    protected float gravity;
    private float _groundSpeed;
    public float GroundSpeed 
    {
        get => isFacingRight ? _groundSpeed : -_groundSpeed;
        
        set => _groundSpeed = Math.Abs(value);
    }
    private float _airSpeed;
    public float AirSpeed 
    {
        get => isFacingRight ? _airSpeed : -_airSpeed;
        
        set => _airSpeed = Math.Abs(value);
    }
    private float _chaseSpeed;
    public float ChaseSpeed 
    {
        get => isFacingRight ? _chaseSpeed : -_chaseSpeed;
        
        set => _chaseSpeed = Math.Abs(value);
    }
    protected float chaseDistance;
    #endregion

    protected abstract void BasicAttack();


    // private void OnTriggerEnter2D(Collider2D item)
    // {
    //     if (item.tag == PlayerAttackTag)
    //     {
    //         GetHit(item);
    //     }
    // }




}