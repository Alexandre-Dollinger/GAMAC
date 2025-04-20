using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Player.Trigger
{
    public class CustomTrigger : MonoBehaviour
    {
        private int GroundLayerId = 3;
        
        private string PlayerTag = "Player";
        private string EnemyTag = "Enemy";

        public event System.Action<Collider2D> EnteredTrigger;
        public event System.Action<Collider2D> ExitedTrigger;

        private void OnTriggerEnter2D(Collider2D item)
        {
            //EnteredTrigger.Invoke(item);
            if (NeedCollision(item)) 
                EnteredTrigger.Invoke(item);
        }

        private void OnTriggerExit2D(Collider2D item)
        {
            //ExitedTrigger.Invoke(item);
            if (NeedCollision(item))
                ExitedTrigger.Invoke(item);
        }

        private bool NeedCollision(Collider2D item)
        {
            return item.tag == PlayerTag || item.tag == EnemyTag || item.gameObject.layer == GroundLayerId; // long line to access the layer of the collider2D
        }
    }
}