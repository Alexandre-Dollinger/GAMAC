using UnityEngine;

namespace _Scripts.Health
{
    public class PlayerHp : UnitHp
    {
        private Transform _playerTransform;
    
        public void Awake()
        {
            _playerTransform = this.transform.parent.parent.transform;
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
    }
}
