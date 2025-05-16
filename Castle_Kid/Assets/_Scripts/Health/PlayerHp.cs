using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Zones;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using _Scripts.GameManager;
using Unity.VisualScripting;
using _Scripts.Multiplayer;

namespace _Scripts.Health
{
    public class PlayerHp : UnitHp
    {
        // my sensei : https://www.youtube.com/watch?v=3yuBOB3VrCk&t=232s
        
        private Transform _playerTransform;
        [SerializeField] private UI _playerUI;

        [CanBeNull] public CheckpointScript currentCheckpoint;
        public override void OnNetworkSpawn()
        {
            _playerTransform = transform.parent.parent.transform;
            
            if (!IsOwner)
            {
                _playerUI.gameObject.SetActive(false);
            }
            
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

            UpdateHeartsServerRpc(OwnerClientId, CurrentHp);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        
            UpdateHeartsServerRpc(OwnerClientId, CurrentHp);
        }

        public override void GainFullLife()
        {
            base.GainFullLife();

            UpdateHeartsServerRpc(OwnerClientId, CurrentHp);
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

        [ServerRpc]
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
            if (currentCheckpoint == null)
                _playerTransform.position = Vector3.zero;
            else
                _playerTransform.position = currentCheckpoint.GetPosition();
        }

        [ServerRpc(RequireOwnership = false)]
        public void GainFullLifeServerRpc()
        {
            GainFullLife();
        }

        [ClientRpc]
        private void UpdateHeartsClientRpc(int playerHealth, ClientRpcParams clientRpcParams)
        {
            _playerUI.UpdateHeartsState(playerHealth);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateHeartsServerRpc(ulong playerId, int playerHealth)
        {
            UpdateHeartsClientRpc(playerHealth, new ClientRpcParams{ Send = new ClientRpcSendParams { TargetClientIds = new List<ulong>{ playerId }}});
        }
    }
}
