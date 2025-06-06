using _Scripts.GameManager;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Player.Trigger
{
    public class CustomTrigger : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
        }
        
        public event System.Action<Collider2D> EnteredTrigger;
        public event System.Action<Collider2D> ExitedTrigger;

        private void OnTriggerEnter2D(Collider2D item)
        {
            //EnteredTrigger.Invoke(item);
            if (NeedCollision(item))
                EnteredTrigger!.Invoke(item);
        }

        private void OnTriggerExit2D(Collider2D item)
        {
            //ExitedTrigger.Invoke(item);
            if (NeedCollision(item))
                ExitedTrigger!.Invoke(item);
        }

        private bool NeedCollision(Collider2D item)
        {
            return item.CompareTag(GM.PlayerTag) ||
                   item.CompareTag(GM.EnemyTag) ||
                   item.gameObject.layer == GM.GroundLayerId; // long line to access the layer of the collider2D
        }
    }
}