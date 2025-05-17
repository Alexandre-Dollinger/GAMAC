using UnityEngine;
using _Scripts.Player.Trigger;
using _Scripts.Enemy;
using System;
using _Scripts.Player.Movement;
using _Scripts.Health;
using _Scripts.GameManager;

public class Slime : BasicEnemy
{
    #region Variables
    public EnemyCustomTrigger BodyCheckTrigger;
    public EnemyCustomTrigger WillFallCheckTrigger;
    public EnemyCustomTrigger WallCheckTrigger;
    public CustomTrigger AirborneCheckTrigger;

    private SpriteRenderer activeSprite;
    private bool isTouchingWall;
    #endregion

    #region Updates and Start
    void Awake()
    {
        enemyType = EnemyType.Slime;
        closestPlayerTime = 0f;
        closestPlayerCooldown = 1f;

        //Look at the SetAllFunctions region for the functions
        SetAllCombatStats();
        SetAllMovementStats();
        SetAllBooleans();
        SetAllTriggers();
        SetAllTriggerConditions();
    }

    void Start()
    {
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
            Chasing();
        else
            Roaming();

        // DebugState();
    }

    void Update()
    {
        if (!GM.GameStarted)
            return;
        CountTimers();
    }

    #endregion

    #region Roaming and Chasing
    private void Roaming()
    {
        activeSprite.color = Color.white;
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
                targetVelocity = new Vector2(MaxGroundSpeed, gravity);
                // enemyRb.linearVelocity = new Vector2(GroundSpeed, gravity);
                acceleration = GroundAcceleration;
            }

            enemyRb.linearVelocity = Vector2.Lerp(enemyRb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }

        //enemyRb.linearVelocity = new Vector2(moveVelocity.x, moveVelocity.y);
    }

    private void Chasing()
    {
        activeSprite.color = Color.red;
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

    private bool InChasingRange()
    {
        return Vector2.Distance(closestPlayer.transform.position, gameObject.transform.position) <= chaseDistance;
    }

    private void UpdateChasingMode()
    {
        if (closestPlayerCooldown <= 0 || !isChasing)
        {
            closestPlayerCooldown = closestPlayerTime;

            closestPlayer = GM.playerTracking.GetClosestPlayer(gameObject);
            //Debug.Log($"Distance: {Vector2.Distance(closestPlayer.transform.position, this.gameObject.transform.position)}");
            isChasing = InChasingRange();

            if (isChasing)
                closestPlayerMovement = closestPlayer.GetComponent<PlayerMovement>();
        }
    }
    #endregion

    protected override void BasicAttack()
    {
        throw new NotImplementedException();
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

    #region SetAllFunctions

    private void SetAllCombatStats()
    {
        AttackPower = 1;
    }
    private void SetAllMovementStats()
    {
        MaxGroundSpeed = 100f;
        GroundAcceleration = 50f ;

        MaxAirSpeed = MaxGroundSpeed * 0.75f;
        AirAcceleration = 70f;

        MaxChaseSpeed = MaxGroundSpeed * 1.5f;
        chaseDistance = 125f; //Can be changed later if needed
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
        BodyCheckTrigger.EnteredTrigger += OnColliderEnter2D;

        WillFallCheckTrigger.EnteredTrigger += OnWillFallCheckEnter2D;
        WillFallCheckTrigger.ExitedTrigger += OnWillFallCheckExit2D;    

        WallCheckTrigger.EnteredTrigger += OnWallCheckEnter2D;
        WallCheckTrigger.ExitedTrigger += OnWallCheckExit2D;

        AirborneCheckTrigger.EnteredTrigger += OnIsGroundedCheckEnter2D;
        AirborneCheckTrigger.ExitedTrigger += OnIsGroundedCheckExit2D;
    }

    private void SetAllTriggerConditions()
    {
        //Setting all the conditions for the different triggers
        WillFallCheckTrigger.condition = TouchedGround();
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
        Debug.Log("Grounded Entered");
        isGrounded = true;
    }
    private void OnIsGroundedCheckExit2D(Collider2D item)
    {
        Debug.Log("Grounded Exited");
        isGrounded = false;
    }

    private void OnColliderEnter2D(Collider2D other) //For detecting if the slime collided with a player
    {
         if (GM.IsPlayer(other))
        {
            Debug.Log("Collision with Player");
            IUnitHp otherHp = other.GetComponent<IUnitHp>();
            otherHp.TakeDamage(AttackPower);
        }
    }

    #endregion
}
