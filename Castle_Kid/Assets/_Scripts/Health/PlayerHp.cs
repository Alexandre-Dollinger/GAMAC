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
        [SerializeField] private UI _playerUI;
        public override void OnNetworkSpawn()
        {
            _playerTransform = transform.parent.parent.transform;
            
            if (IsOwner)
                GM.playerTracking.SetPlayerList();
            else
                GM.playerTracking.PlayerList.Add(transform.parent.parent.gameObject);
            
            if (IsServer)
            {
                MaxHp = 100;
                CurrentHp = MaxHp;
            }
        }

        public void Update()
        {
            if (CurrentHp <= 0)
                    Die();
        }

        public override void GainHealth(int healthGained)
        {
            base.GainHealth(healthGained);
            _playerUI.UpdateHeartsState();
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            _playerUI.UpdateHeartsState();
        }

        public override void GainFullLife()
        {
            base.GainFullLife();
            _playerUI.UpdateHeartsState();
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
                GainFullLife();
            DieClientRpc(new ClientRpcParams{ Send = new ClientRpcSendParams { TargetClientIds = new List<ulong>{ serverRpcParams.Receive.SenderClientId }}});
            // That line above is my baby 
        }
        
        private void DieLocally()
        {
            _playerTransform.position = Vector3.zero;
        }
    }
}
