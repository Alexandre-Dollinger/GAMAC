using UnityEngine;
using _Scripts.Enemy;
using _Scripts.Player.Weapon;
using _Scripts.Player;
public class Slime : BasicEnemy
{

    public EnemyCustomTrigger GroundCheckTrigger;
    public EnemyCustomTrigger WallCheckTrigger;
    public EnemyCustomTrigger AirborneCheckTrigger;

    private bool isTouchingWall;

    #region Updates and Start
    void Awake()
    {
        MaxHp = 100;
        Hp = MaxHp;
        AttackPower = 1;

        GroundSpeed = 50f;
        AirSpeed = GroundSpeed / 2;

        isFacingRight = true; 
        isGrounded = false; 
        isTouchingWall = false;
        isAirborne = true;



        //Setting all the conditions for the different triggers
        GroundCheckTrigger.condition = item => groundLayerId == item.gameObject.layer;

        WallCheckTrigger.condition = item =>  groundLayerId == item.gameObject.layer;

        AirborneCheckTrigger.condition = item => groundLayerId == item.gameObject.layer;
        
        // Setting all the Enter and Exit triggers for the different triggers
        GroundCheckTrigger.EnteredTrigger += OnGroundCheckEnter2D;
        GroundCheckTrigger.ExitedTrigger += OnGroundCheckExit2D;

        WallCheckTrigger.EnteredTrigger += OnWallCheckEnter2D;
        WallCheckTrigger.ExitedTrigger += OnWallCheckExit2D;

        AirborneCheckTrigger.EnteredTrigger += OnAirboneCheckEnter;
        AirborneCheckTrigger.ExitedTrigger += OnAirboneCheckExit;
    }

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        if (!isGrounded)
            {
                if(isAirborne) 
                    enemyRb.linearVelocity = new Vector2(AirSpeed, -50);
                else 
                {
                    Flip();
                    enemyRb.linearVelocity = new Vector2(GroundSpeed, 0);
                }
            }
        else if (isTouchingWall)
            {
                //isTouchingWall = false;
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


    #region Collisions and Triggers

    private void OnGroundCheckEnter2D(Collider2D item)
    {
        isGrounded = true;
    }
    private void OnGroundCheckExit2D(Collider2D item)
    {
        isGrounded = false;
    }

    private void OnWallCheckEnter2D(Collider2D item)
    {
        isTouchingWall = true; 
    }
    private void OnWallCheckExit2D(Collider2D item)
    {
        isTouchingWall = false; 
    }

    private void OnAirboneCheckEnter(Collider2D item)
    {
        isAirborne = false; 
    }
    private void OnAirboneCheckExit(Collider2D item)
    {
        isAirborne = true; 
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

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);

        GroundSpeed = -GroundSpeed;
        AirSpeed = -AirSpeed;
    }

    
}
