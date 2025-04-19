using UnityEngine;

public class Slime : BasicEnemy
{
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


    #region Collisions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == PlayerAttackTag) //Doesn't work yet
            GetHit(10);
    }
    private void OnTriggerEnter2D(Collider2D item)
    {
        if (item.gameObject.layer == groundLayerId)
            isGrounded = true;
    }
    private void OnTriggerExit2D(Collider2D item)
    {
        if (item.gameObject.layer == groundLayerId)
            isGrounded = false;
    }

    #endregion

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
        groundSpeed = -groundSpeed;
    }

    #region Updates and Start
    
    void Awake()
    {
        MaxHp = 30;
        Hp = MaxHp;
        groundSpeed = 50f;
        isFacingRight = true;
    }

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        if (!isGrounded)
            {
                enemyRb.linearVelocity = new Vector2(groundSpeed, -50);
                Flip();
            }
        else 
            enemyRb.linearVelocity = new Vector2(groundSpeed, 0);
    }
    void Update()
    {
        
    }

    #endregion
}
