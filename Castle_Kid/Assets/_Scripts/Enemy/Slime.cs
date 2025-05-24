using UnityEngine;
using _Scripts.Player.Trigger;
using _Scripts.Enemy;
using System;
using _Scripts.Player.Movement;
using _Scripts.Health;
using _Scripts.GameManager;
using _Scripts.Player.PowerUp;
using _Scripts.Projectiles;
using UnityEditor.Animations;

public class Slime : BasicEnemy
{
    #region Variables
    private bool isTouchingWall;
    [SerializeField] private Collider2D body;
    public EnemyCustomTrigger WallCheckTrigger;
    public EnemyCustomTrigger WillFallTrigger;
    public CustomTrigger FeetTrigger;
    private SpriteRenderer activeSprite;

    private Animator slimeAnimator;
    private string _curAnimationState;
    private float _animationTime;
    private float _healthSaved;
    private BasicEnemyHp basicEnemyHp;

    #endregion

    #region Updates and Start
    void Awake()
    {
        enemyType = EnemyType.Slime;
        closestPlayerTime = 0f;
        closestPlayerCooldown = 1f;

        slimeAnimator = GetComponent<Animator>();
        basicEnemyHp = GetComponentInChildren<BasicEnemyHp>();

        //Look at the SetAllFunctions region for the functions
        SetAllCombatStats();
        SetAllMovementStats();
        SetAllBooleans();
        SetAllTriggers();
        SetAllTriggerConditions();
    }

    void Start()
    {
        _healthSaved = basicEnemyHp.CurrentHp;
        enemyRb = GetComponent<Rigidbody2D>();
        activeSprite = GetComponent<SpriteRenderer>();
    }

    private void DebugState()
    {
        Debug.Log(isChasing ? "Chasing" : "Roaming");
    }

    void FixedUpdate()
    {
        if (!GM.GameStarted)
            return;

        UpdateChasingMode();
        CheckFlip();

        if (isChasing)
        {
            Chasing();
            if (powerUp.ReadyToFire())
                powerUp.Fire(body.bounds.center, closestPlayer.transform.position - body.bounds.center, GM.EnemyProjectileTag, -1);
        }
        else
            Roaming();

        // DebugState();
    }

    void Update()
    {
        if (!GM.GameStarted)
            return;
        CountTimers();
        CheckAnimation();
    }

    #endregion

    #region Roaming and Chasing
    private void Roaming()
    {
        //Vector2 moveVelocity = enemyRb.linearVelocity;
        Vector2 targetVelocity;
        float acceleration;

        if (!isGrounded)
        {
            targetVelocity = new Vector2(MaxAirSpeed, gravity);
            //enemyRb.linearVelocity = new Vector2(AirSpeed, gravity);
            acceleration = AirAcceleration;
        }

        else
        {
            targetVelocity = new Vector2(MaxGroundSpeed, gravity);
            // enemyRb.linearVelocity = new Vector2(GroundSpeed, gravity);
            acceleration = GroundAcceleration;
        }

        enemyRb.linearVelocity = Vector2.Lerp(enemyRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        //enemyRb.linearVelocity = new Vector2(moveVelocity.x, moveVelocity.y);
    }

    private void Chasing()
    {
        //Vector2 moveVelocity = enemyRb.linearVelocity;
        Vector2 targetVelocity;
        float acceleration;

        if (willFall)
        {
            if (!isGrounded)
            {
                targetVelocity = new Vector2(MaxAirSpeed, gravity);
                //enemyRb.linearVelocity = new Vector2(AirSpeed, gravity);
                acceleration = AirAcceleration;
            }

            else
            {
                targetVelocity = new Vector2(MaxChaseSpeed, gravity);
                //enemyRb.linearVelocity = new Vector2(ChaseSpeed, gravity);
                acceleration = GroundAcceleration;
            }

            enemyRb.linearVelocity = Vector2.Lerp(enemyRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else if (isTouchingWall)
        {
            // Debug.Log("Wall Touched in Chasing Mode");
            // enemyRb.linearVelocity = new Vector2(0, -50);
            enemyRb.linearVelocity = new Vector2(0, 0);
        }
        else
        {
            targetVelocity = new Vector2(MaxChaseSpeed, gravity);
            acceleration = GroundAcceleration;

            enemyRb.linearVelocity = Vector2.Lerp(enemyRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            // enemyRb.linearVelocity = new Vector2(ChaseSpeed, gravity);
        }

        // enemyRb.linearVelocity = new Vector2(moveVelocity.x, moveVelocity.y);
    }
    #endregion
    private bool InChasingRange()
    {
        return Vector2.Distance(closestPlayer.transform.position, body.bounds.center) <= chaseDistance;
    }

    private void UpdateChasingMode()
    {
        if (closestPlayerCooldown <= 0 || !isChasing)
        {
            closestPlayerCooldown = closestPlayerTime;

            try
            {
                closestPlayer = GM.playerTracking.GetClosestPlayer(gameObject);
            }
            catch (Exception)
            {
                GM.playerTracking.CheckAndRemoveNull();
                closestPlayer = GM.playerTracking.GetClosestPlayer(gameObject);
            }

            //Debug.Log($"Distance: {Vector2.Distance(closestPlayer.transform.position, this.gameObject.transform.position)}");
            isChasing = InChasingRange();

            if (isChasing)
                closestPlayerMovement = closestPlayer.GetComponent<PlayerMovement>();
        }
    }

    private bool PlayerIsRight()
    {
        return transform.position.x < closestPlayer.transform.position.x;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);

        //GroundSpeed = -GroundSpeed;
        //AirSpeed = -AirSpeed;
        //ChaseSpeed = -ChaseSpeed;
    }

    private void CheckFlip()
    {
        if (isChasing) //Chasing
        {
            if (PlayerIsRight() ^ isFacingRight) //&& closestPlayerMovement.GetPlayerIsGrounded())  // '^' symbol --> XOR
                Flip();
            //Player on a side and Facing the other side
            //!PlayerIsRight() && isFacingRight || PlayerIsRight() && !isFacingRight
        }
        else //Roaming
        {
            if ((willFall && isGrounded) || isTouchingWall) // isTouching
                Flip();
        }
    }

    #region Attack 
    protected override void SetPowerUp1()
    {
        powerUp = new PowerUpStruct(GM.GetLinearProjectileStruct(), ProjectilePrefabs.BubblesCone, 2f);
        powerUp.ProjStruct.TargetingEnemy = false;
        powerUp.ProjStruct.TargetingEnemyProjectile = false;

        powerUp.ProjStruct.Damage = 10;
        powerUp.ProjStruct.Speed = 100;

        powerUp.ProjStruct.InitDestroyCondition(true, 50, false, false);
    }

    protected override void SetPowerUp2()
    {
        powerUp = new PowerUpStruct(GM.GetTrackingProjectileStruct(), ProjectilePrefabs.MagicArrowCone, 4f);
        powerUp.ProjStruct.TargetingEnemy = false;
        powerUp.ProjStruct.TargetingEnemyProjectile = false;

        powerUp.ProjStruct.Damage = 30;
        powerUp.ProjStruct.Speed = 70;

        powerUp.ProjStruct.InitDestroyCondition(true, 50, true, false);
    }

    protected override void SetPowerUp3()
    {
        powerUp = new PowerUpStruct(GM.GetTrackingProjectileStruct(), ProjectilePrefabs.FireCone, 6f);
        powerUp.ProjStruct.TargetingEnemy = false;
        powerUp.ProjStruct.TargetingEnemyProjectile = false;

        powerUp.ProjStruct.Damage = 50;
        powerUp.ProjStruct.Speed = 50;

        powerUp.ProjStruct.InitDestroyCondition(true, 200, true, false);
    }

    #endregion


    #region SetAllFunctions
    private void SetAllCombatStats()
    {
        System.Random random = new System.Random();
        int randomPowerUp = random.Next(1, 4);
        switch (randomPowerUp)
        {
            case 1:
                SetPowerUp1();
                break;
            case 2:
                SetPowerUp2();
                break;
            case 3:
                SetPowerUp3();
                break;
        }
    }
    private void SetAllMovementStats()
    {
        MaxGroundSpeed = 100f;
        GroundAcceleration = 50f;

        MaxAirSpeed = MaxGroundSpeed * 0.75f;
        AirAcceleration = 70f;

        MaxChaseSpeed = MaxGroundSpeed * 1.5f;
        chaseDistance = 200f; //Can be changed later if needed
    }
    private void SetAllBooleans()
    {
        isFacingRight = true;
        willFall = true;
        isTouchingWall = false;
        isGrounded = false;
        isChasing = false;
    }

    private void SetAllTriggers()
    {
        // Setting all the Enter and Exit triggers for the different triggers

        FeetTrigger.EnteredTrigger += OnIsGroundedCheckEnter2D;
        FeetTrigger.ExitedTrigger += OnIsGroundedCheckExit2D;

        WillFallTrigger.EnteredTrigger += OnWillFallCheckEnter2D;
        WillFallTrigger.ExitedTrigger += OnWillFallCheckExit2D;

        WallCheckTrigger.EnteredTrigger += OnWallCheckEnter2D;
        WallCheckTrigger.ExitedTrigger += OnWallCheckExit2D;
    }

    private void SetAllTriggerConditions()
    {
        //Setting all the conditions for the different triggers
        WillFallTrigger.condition = TouchedGround();
        WallCheckTrigger.condition = item => TouchedGround()(item) || TouchedEnemy()(item);
        //AirborneCheckTrigger.condition = item => groundLayerId == item.gameObject.layer;
    }
    #endregion

    #region Predicates (Conditions)

    private Predicate<Collider2D> TouchedGround()
    {
        return item => GM.CrossedWall(item);
    }

    private Predicate<Collider2D> TouchedEnemy()
    {
        return item => GM.IsEnemy(item);
    }

    #endregion

    #region Timer

    private void CountTimers()
    {
        float deltaTime = Time.deltaTime;

        if (closestPlayerCooldown > 0)
        {
            closestPlayerCooldown -= deltaTime;
        }

        powerUp.CountTimer(deltaTime);
    }

    #endregion

    #region Collisions and Triggers
    private void OnWillFallCheckEnter2D(Collider2D item)
    {
        willFall = false;
    }
    private void OnWillFallCheckExit2D(Collider2D item)
    {
        willFall = true;
    }

    private void OnWallCheckEnter2D(Collider2D item)
    {
        isTouchingWall = true;
    }
    private void OnWallCheckExit2D(Collider2D item)
    {
        isTouchingWall = false;
    }

    private void OnIsGroundedCheckEnter2D(Collider2D item)
    {
        isGrounded = true;
    }
    private void OnIsGroundedCheckExit2D(Collider2D item)
    {
        isGrounded = false;
    }
    #endregion

    #region Animation

    private void ChangeAnimationState(string newState, float time = 0)
    {
        if (_curAnimationState == newState) return;

        if (_animationTime <= 0)
        {
            if (time != 0)
            {
                _animationTime = time;
            }

            slimeAnimator.Play(newState);

            _curAnimationState = newState;
        }
    }

    private void CheckAnimation()
    {
        if (_healthSaved != basicEnemyHp.CurrentHp)
        {
            _healthSaved = basicEnemyHp.CurrentHp;
            ChangeAnimationState("HurtAnimation", 0.4f);
        }
        else if (isChasing)
            ChangeAnimationState("RunAnimation");
        else
            ChangeAnimationState("WalkAnimation");
    }

    #endregion
    }
