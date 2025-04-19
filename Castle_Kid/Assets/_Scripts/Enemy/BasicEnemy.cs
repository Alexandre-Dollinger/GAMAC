using UnityEngine;
using Unity.Netcode;

public abstract class BasicEnemy : MonoBehaviour
{

    //Base class that all enemies will inherit from

    protected Rigidbody2D enemyRb;
    public GameObject groundCheck;
    protected int groundLayerId = 3;

    protected bool isFacingRight;
    protected bool isGrounded;


    //Combat Stats
    protected int MaxHp {get; set;}
    protected int Hp {get; set;}
    protected int AttackPower {get; set;} //How much damage he deals per attack
    
    //Drops
    protected int MoneyDrop {get;}
    protected bool IsGuardian {get;} //Rare enemy

    //Movement Stats
    public float MaxGroundSpeed {get;}
    public float groundSpeed {get; set;}
    public float MaxAirSpeed {get;}
    public float AirSpeed {get; set;}

    //Player Attack Tag
    protected string PlayerAttackTag = "PlayerAttack";

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