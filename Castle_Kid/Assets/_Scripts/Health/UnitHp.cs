using UnityEngine;

namespace _Scripts.Health
{
    public class UnitHp : MonoBehaviour, IUnitHp // the script of the Hp must be added where the collider of the body of the object is
    { // maybe need NetworkBehaviour instead
        public bool CanDie { get; set; } = true;

        private int _currentHp;
        public int CurrentHp
        {
            get => _currentHp;
            set
            {
                if (value < 0)
                    _currentHp = 0;

                if (value > MaxHp)
                    _currentHp = MaxHp;

                _currentHp = value; 
            }
        }

        private int _maxHp;
        public int MaxHp
        {
            get => _maxHp;
            set
            {
                if (value < 0)
                {
                    _maxHp = 0;
                }

                _maxHp = value;
            }
        }

        public void TakeDamage(int damage)
        {
            if (CanDie)
                CurrentHp -= damage;
        }

        public void GainHealth(int healthGained)
        {
            CurrentHp += healthGained;
        }

        public virtual void Die()
        {
            CurrentHp = 0;
        }

        public virtual void GainFullLife()
        {
            CurrentHp = MaxHp;
        }
    }
}
