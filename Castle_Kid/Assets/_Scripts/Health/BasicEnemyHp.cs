using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Health
{
    public class BasicEnemyHp : UnitHp
    {
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                MaxHp = 100;
                CurrentHp = MaxHp;
            }
        }

        void Awake()
        {
           //transform.parent.parent.GetComponent<NetworkObject>().Spawn();
        }

        public void Update()
        {
            if (CurrentHp <= 0)
                Die();
        }
        
        public override void Die()
        {
            if (IsOwner)
                DieServerRpc();
        }

        [ClientRpc]
        private void DieClientRpc(ClientRpcParams clientRpcParams)
        {
            DieLocally();
        }

        [ServerRpc(RequireOwnership = false)]
        private void DieServerRpc(ServerRpcParams serverRpcParams = default)
        {
            //DieLocally();
  
            DieClientRpc(new ClientRpcParams{ Send = new ClientRpcSendParams { TargetClientIds = new List<ulong>{ serverRpcParams.Receive.SenderClientId }}});
            // That line above is my baby 
        }
        
        private void DieLocally()
        {
            Debug.Log("EnemyDied");
            Destroy(transform.parent.parent.gameObject);
        }
    }
}