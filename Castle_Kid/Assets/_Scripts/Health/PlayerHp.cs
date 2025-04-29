using UnityEngine;

namespace _Scripts.Health
{
    public class PlayerHp : UnitHp
    {
        private Transform _playerTransform;
    
        public void Awake()
        {
            _playerTransform = GetComponent<Transform>();
            MaxHp = 100;
            CurrentHp = MaxHp;
        }

        public void Update()
        {
            if (CurrentHp <= 0)
                Die();
        }

        public override void Die()
        {
            // Do something to the dead player
            _playerTransform.position = Vector3.zero;
            CurrentHp = MaxHp;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GM.EnemyTag) ||
                other.CompareTag(GM.EnemyProjectileTag))
            {
                TakeDamage(25); // need to be changed to the enemy attack
            }
        }
    }
}
