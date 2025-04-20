using System;
using UnityEngine;

namespace _Scripts.Enemy
{
    public class EnemyCustomTrigger : MonoBehaviour
    {
        //Function that returns a boolean value to check if the necessary conditions are met
        public Predicate<Collider2D> condition; 

        public event Action<Collider2D> EnteredTrigger;
        public event Action<Collider2D> ExitedTrigger;

        private void OnTriggerEnter2D(Collider2D item)
        {
            //Debug.Log($"{item.gameObject.name} Trigger Enter");

            if (condition is not null && condition(item))
                EnteredTrigger.Invoke(item);
        }

        private void OnTriggerExit2D(Collider2D item)
        {
            //Debug.Log($"{item.gameObject.name} Trigger Exit");

            if (condition is not null && condition(item))
                ExitedTrigger.Invoke(item);
        }

       

    }
}