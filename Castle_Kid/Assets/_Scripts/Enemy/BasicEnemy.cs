using UnityEngine;
using System;
using _Scripts.Player.Movement;
using Unity.Netcode;
using _Scripts.GameManager;
using _Scripts.Enemy;
using _Scripts.Health;
using _Scripts.Player.PowerUp;

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
    public PowerUpStruct powerUp;

    //Drops
    public int MoneyDrop { get; }
    public bool IsGuardian { get; } //Rare enemy

    #region Movement Stats
    protected float gravity = -200f;
    protected float GroundAcceleration;
    protected float AirAcceleration;

    private float _maxGroundSpeed;
    public float MaxGroundSpeed
    {
        get => isFacingRight ? _maxGroundSpeed : -_maxGroundSpeed;

        set => _maxGroundSpeed = Math.Abs(value);
    }
    private float _maxAirSpeed;
    public float MaxAirSpeed
    {
        get => isFacingRight ? _maxAirSpeed : -_maxAirSpeed;

        set => _maxAirSpeed = Math.Abs(value);
    }
    private float _maxChaseSpeed;
    public float MaxChaseSpeed
    {
        get => isFacingRight ? _maxChaseSpeed : -_maxChaseSpeed;

        set => _maxChaseSpeed = Math.Abs(value);
    }
    protected float chaseDistance;
    #endregion

    protected abstract void SetPowerUp1();
    protected abstract void SetPowerUp2();
    protected abstract void SetPowerUp3();

    // private void OnTriggerEnter2D(Collider2D item)
    // {
    //     if (item.tag == PlayerAttackTag)
    //     {
    //         GetHit(item);
    //     }
    // }




}