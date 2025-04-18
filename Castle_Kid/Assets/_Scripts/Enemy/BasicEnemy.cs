using UnityEngine;
using Unity.Netcode;

public abstract class BasicEnemy
{

    //Base class that all enemies will inherit from


    //Combat Stats
    protected int MaxHp {get;}
    protected int Hp {get; set;}
    protected int AttackPower {get; set;} //How much damage he deals per attack
    
    //Drops
    protected int MoneyDrop {get;}
    protected bool IsGuardian {get;} //Rare enemy

    //Movement Stats
    protected float MaxGroundSpeed {get;}
    protected float MaxAirSpeed {get;}

    //Player Attack Tag
    protected string PlayerAttackTag = "PlayerAttack";

    protected abstract void BasicAttack();

    protected abstract void GetHit(Collider2D playerAttack);

    private void OnTriggerEnter2D(Collider2D item)
    {
        if (item.tag == PlayerAttackTag)
        {
            GetHit(item);
        }
    }




}