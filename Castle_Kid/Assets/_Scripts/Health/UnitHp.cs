using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Health
{
    public class UnitHp : NetworkBehaviour, IUnitHp // the script of the Hp must be added where the collider of the body of the object is
    { // maybe need NetworkBehaviour instead
        public bool CanDie { get; set; } = true;

        private NetworkVariable<int> _currentHp = new NetworkVariable<int>(); // https://www.youtube.com/watch?v=3yuBOB3VrCk&t=232s
        public int CurrentHp
        {
            get => _currentHp.Value;
            set
            {
                if (value < 0)
                    _currentHp.Value = 0;

                else if (value > MaxHp)
                    _currentHp.Value = MaxHp;

                else 
                    _currentHp.Value = value; 
            }
        }
        /*
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
        */

        private NetworkVariable<int> _maxHp = new NetworkVariable<int>();
        public int MaxHp
        {
            get => _maxHp.Value;
            set
            {
                if (value < 0)
                    _maxHp.Value = 0;
                
                else 
                    _maxHp.Value = value;
            }
        }
        /*
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
        */

        public virtual void TakeDamage(int damage)
        {
            if (CanDie)
                CurrentHp -= damage;
        }

        public virtual void GainHealth(int healthGained)
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

        public bool IsNetwork => true;
    }
}
