using UnityEngine;
using _Scripts.Enemy;
using _Scripts.Player.Weapon;
using _Scripts.Player;
public class Slime : BasicEnemy
{
    public EnemyCustomTrigger GroundCheckTrigger;
    public EnemyCustomTrigger WallCheckTrigger;
    public EnemyCustomTrigger AirborneCheckTrigger;

    public SpriteRenderer activeSprite;
    private bool isTouchingWall;

    #region Updates and Start
    void Awake()
    {
        MaxHp = 100;
        Hp = MaxHp;
        AttackPower = 1;

        chaseDistance = 125f; //Can be changed later if needed

        GroundSpeed = 40f;
        AirSpeed = GroundSpeed / 2;
        ChaseSpeed = 75f;

        isFacingRight = true; 
        willFall = true; 
        isTouchingWall = false;
        isGrounded = false;
        isChasing = false;

        //Setting all the conditions for the different triggers
        GroundCheckTrigger.condition = item => groundLayerId == item.gameObject.layer;

        WallCheckTrigger.condition = item =>  groundLayerId == item.gameObject.layer;

        AirborneCheckTrigger.condition = item => groundLayerId == item.gameObject.layer;
        
        // Setting all the Enter and Exit triggers for the different triggers
        GroundCheckTrigger.EnteredTrigger += OnGroundCheckEnter2D;
        GroundCheckTrigger.ExitedTrigger += OnGroundCheckExit2D;

        WallCheckTrigger.EnteredTrigger += OnWallCheckEnter2D;
        WallCheckTrigger.ExitedTrigger += OnWallCheckExit2D;

        AirborneCheckTrigger.EnteredTrigger += OnIsGroundedCheckEnter;
        AirborneCheckTrigger.ExitedTrigger += OnIsGroundedCheckExit;
    }

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        activeSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        closestPlayer = PlayerTracking.GetClosestPlayer(this.gameObject);
        //Debug.Log($"Distance: {Vector2.Distance(closestPlayer.transform.position, this.gameObject.transform.position)}");
        isChasing = InChasingRange();
        if (isChasing)
        {
            Debug.Log("CHASING");
            Chasing();
        }
        else 
        {
            Debug.Log("ROAMING");
            Roaming();
        }
    }
    
    #endregion

    #region Collisions and Triggers

    private void OnGroundCheckEnter2D(Collider2D item)
    {
        willFall = false;
    }
    private void OnGroundCheckExit2D(Collider2D item)
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

    private void OnIsGroundedCheckEnter(Collider2D item)
    {
        isGrounded = true; 
    }
    private void OnIsGroundedCheckExit(Collider2D item)
    {
        isGrounded = false; 
    }

    private void OnTriggerEnter2D(Collider2D item) //For detecting if the slime got attacked by a player
    {
        if (item.gameObject.tag == playerAttackTag) //Will need to implement a timer to avoid continuous hits
        {
            Debug.Log($"Got hit ! Current HP: {Hp}/{MaxHp}");
            GetHit(WeaponScript.AttackPower);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) //For detecting if the slime collided with a player
    {
        if (collision.gameObject.tag == playerTag)
        {
            Debug.Log("Collision with Player");
            collision.gameObject.GetComponent<PlayerStats>().GetHit(AttackPower);
        }
    }

    #endregion

    protected override void BasicAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void GetHit(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
            Destroy(this.gameObject);
    }

    private void Roaming()
    {
        activeSprite.color = Color.white;
        if (willFall)
        {
                if(!isGrounded) 
                    enemyRb.linearVelocity = new Vector2(AirSpeed, -50);
                else 
                {
                    Flip();
                    enemyRb.linearVelocity = new Vector2(GroundSpeed, 0);
                }
        }
        else if (isTouchingWall)
        {
                Flip();
        }
        
        else 
        {
            //Feels weird but works, percentage can be changed

            // if (Random.Range(0f, 100f) < 0.5) 
            //     Flip();

            enemyRb.linearVelocity = new Vector2(GroundSpeed, 0);
        }
    }

    private void Chasing()
    {
        activeSprite.color = Color.red;
        if (PlayerIsRight() ^ isFacingRight)  // '^' symbol --> XOR
        //Player on a side and Facing the other side
        //!PlayerIsRight() && isFacingRight || PlayerIsRight() && !isFacingRight
        {
            Flip();
        }

        if (willFall)
        {
            if(!isGrounded) 
                enemyRb.linearVelocity = new Vector2(AirSpeed, -50);
            else 
            {
                enemyRb.linearVelocity = new Vector2(0, 0);
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

    private bool PlayerIsRight()
    {
        return transform.position.x < closestPlayer.transform.position.x;
    }

    private bool InChasingRange()
    {
        return Vector2.Distance(closestPlayer.transform.position, gameObject.transform.position) <= chaseDistance;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);

        GroundSpeed = -GroundSpeed;
        AirSpeed = -AirSpeed;
        ChaseSpeed = -ChaseSpeed;
    }

    
}
