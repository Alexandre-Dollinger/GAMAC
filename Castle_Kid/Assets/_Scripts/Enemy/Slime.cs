using UnityEngine;
using _Scripts.Player.Weapon;
using _Scripts.Player;
using _Scripts.Player.Trigger;
using _Scripts.Enemy;
using System;
using _Scripts.Player.Movement;
using _Scripts.Health;
using _Scripts.GameManager;
public class Slime : BasicEnemy
{
    public EnemyCustomTrigger BodyCheckTrigger;
    public EnemyCustomTrigger WillFallCheckTrigger;
    public EnemyCustomTrigger WallCheckTrigger;
    public CustomTrigger AirborneCheckTrigger;

    private SpriteRenderer activeSprite;
    private bool isTouchingWall;

    private float targetvelocityX;
    private float targetVelocityY;

    private float GroundAcceleration;
    private float AerialAcceleration;

    #region Updates and Start
    void Awake()
    {
        enemyType = EnemyType.Slime;
        closestPlayerTime = 2f;
        closestPlayerCooldown = 0f;

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
        UpdateChasingMode();
        CheckFlip();

        if (isChasing)
            Chasing();
        else
            Roaming();

        DebugState();
    }

    void Update()
    {
        CountTimers();
    }

    #endregion

    #region Roaming and Chasing
    private void Roaming()
    {
        activeSprite.color = Color.white;
        if (willFall)
        {
            if (!isGrounded)
                enemyRb.linearVelocity = new Vector2(AirSpeed, -50);
            else
                enemyRb.linearVelocity = new Vector2(GroundSpeed, 0);
        }

        else
        {
            enemyRb.linearVelocity = new Vector2(GroundSpeed, 0);
        }
    }

    private void Chasing()
    {
        activeSprite.color = Color.red;

        if (willFall)
        {
            if (!isGrounded)
                enemyRb.linearVelocity = new Vector2(AirSpeed, -50);
            else
            {
                enemyRb.linearVelocity = new Vector2(GroundSpeed, 0);
            }
        }
        else if (isTouchingWall)
        {
            Debug.Log("Wall Touched in Chasing Mode");
            enemyRb.linearVelocity = new Vector2(0, 0);
        }
        else
        {
            enemyRb.linearVelocity = new Vector2(ChaseSpeed, 0);
        }
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

            closestPlayer = PlayerTracking.GetClosestPlayer(gameObject);
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
            if ((PlayerIsRight() ^ isFacingRight))//&& closestPlayerMovement.GetPlayerIsGrounded())  // '^' symbol --> XOR
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
        GroundSpeed = 40f;
        AirSpeed = GroundSpeed / 2;
        ChaseSpeed = 75f;
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
        isGrounded = true;
    }
    private void OnIsGroundedCheckExit2D(Collider2D item)
    {
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
