using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public abstract class BasicEnemy : MonoBehaviour
{

    //Base class that all enemies will inherit from
    protected GameObject closestPlayer;
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
    public float GroundSpeed {get; set;}
    public float AirSpeed {get; set;}
    public float ChaseSpeed {get; set;}
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