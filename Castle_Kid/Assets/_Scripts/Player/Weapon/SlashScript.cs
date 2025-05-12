using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Player.Weapon
{
    public class SlashScript : NetworkBehaviour
    {
        private WeaponScript _weaponScript;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
        }
    
        public void Awake()
        {
            _weaponScript = GetComponentInParent<WeaponScript>();
        }

        private void DisableSlash()
        {
            _weaponScript.DisableCollider();
        }
    }
}
