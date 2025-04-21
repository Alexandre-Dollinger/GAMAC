using UnityEngine;
using System;
using _Scripts.Player.Movement;

public abstract class BasicEnemy : MonoBehaviour
{

    //Base class that all enemies will inherit from
    protected GameObject closestPlayer;
    protected PlayerMovement closestPlayerMovement;
    protected float closestPlayerCooldown; // The one which is updated
    protected float closestPlayerTime; // Max time
    protected Rigidbody2D enemyRb;

    protected bool isFacingRight;
    protected bool willFall;
    protected bool isGrounded;
    protected bool isChasing;


    //Combat Stats
    protected int MaxHp {get; set;}
    protected int Hp {get; set;}
    protected int AttackPower {get; set;} //How much damage the enemy deals per attack
    
    //Drops
    protected int MoneyDrop {get;}
    protected bool IsGuardian {get;} //Rare enemy

    //Movement Stats
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

    //References to Id and tags
    protected int groundLayerId = 3; 

    protected string playerTag = "Player";
    protected string playerAttackTag = "PlayerAttack"; 

    protected abstract void BasicAttack();

    protected abstract void GetHit(int damage);

    // private void OnTriggerEnter2D(Collider2D item)
    // {
    //     if (item.tag == PlayerAttackTag)
    //     {
    //         GetHit(item);
    //     }
    // }




}