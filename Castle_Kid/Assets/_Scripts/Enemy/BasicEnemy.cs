using UnityEngine;
using Unity.Netcode;

public abstract class BasicEnemy : MonoBehaviour
{

    //Base class that all enemies will inherit from

    protected Rigidbody2D enemyRb;

    protected bool isFacingRight;
    protected bool isGrounded;
    protected bool isAirborne;


    //Combat Stats
    protected int MaxHp {get; set;}
    protected int Hp {get; set;}
    protected int AttackPower {get; set;} //How much damage the enemy deals per attack
    
    //Drops
    protected int MoneyDrop {get;}
    protected bool IsGuardian {get;} //Rare enemy

    //Movement Stats
    public float MaxGroundSpeed {get;}
    public float GroundSpeed {get; set;}
    public float MaxAirSpeed {get;}
    public float AirSpeed {get; set;}

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