using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Health
{
    public class PlayerHp : UnitHp
    {
        // my sensei : https://www.youtube.com/watch?v=3yuBOB3VrCk&t=232s
        
        private Transform _playerTransform;

        public override void OnNetworkSpawn()
        {
            _playerTransform = transform.parent.parent.transform;
            MaxHp = 100;
            if (IsServer)
                CurrentHp = MaxHp;
        }

        public void Update()
        {
            if (CurrentHp <= 0)
                    Die();
        }

        [ClientRpc]
        public void DieClientRpc(ClientRpcParams clientRpcParams)
        {
            _playerTransform.position = Vector3.zero;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DieServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerTransform.position = Vector3.zero;
            if (IsServer)
                CurrentHp = MaxHp;
            DieClientRpc(new ClientRpcParams{ Send = new ClientRpcSendParams { TargetClientIds = new List<ulong>{ serverRpcParams.Receive.SenderClientId }}});
            // That line above is my baby 
        }

        public override void Die()
        {
            if (IsOwner)
                DieServerRpc();
        }
    }
}
