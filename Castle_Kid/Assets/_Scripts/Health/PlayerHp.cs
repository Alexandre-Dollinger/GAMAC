using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using _Scripts.GameManager;

namespace _Scripts.Health
{
    public class PlayerHp : UnitHp
    {
        // my sensei : https://www.youtube.com/watch?v=3yuBOB3VrCk&t=232s
        
        private Transform _playerTransform;

        public override void OnNetworkSpawn()
        {
            _playerTransform = transform.parent.parent.transform;
            if (IsServer)
            {
                PlayerTracking.PlayerList.Add(transform.parent.parent.gameObject);
                CurrentHp = MaxHp;
                MaxHp = 100;
            }
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
            DieLocally();
            if (IsServer)
                CurrentHp = MaxHp;
            DieClientRpc(new ClientRpcParams{ Send = new ClientRpcSendParams { TargetClientIds = new List<ulong>{ serverRpcParams.Receive.SenderClientId }}});
            // That line above is my baby 
        }
        
        private void DieLocally()
        {
            _playerTransform.position = Vector3.zero;
        }
    }
}
