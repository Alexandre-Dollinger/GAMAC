using System;
using _Scripts.GameManager;
using _Scripts.Health;
using _Scripts.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Zones
{
    public class CheckpointScript : NetworkBehaviour
    {
        private Animator _flagAnimator;

        public void Awake()
        {
            _flagAnimator = GetComponent<Animator>();
        }

        public Vector3 GetPosition()
        {
            return gameObject.transform.position;
        }

        public void ActivateCheckPoint(PlayerHp playerHp)
        {
            playerHp.GainFullLifeServerRpc();
        
            _flagAnimator.Play("FlagActivatedAnimation");

            if (playerHp.currentCheckpoint is not null && playerHp.currentCheckpoint != this)
                playerHp.currentCheckpoint.DeactivateCheckPoint();

            playerHp.currentCheckpoint = this;
        }

        public void DeactivateCheckPoint()
        {
            _flagAnimator.Play("FlagDeactivatedAnimation");
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GM.PlayerTag) && 
                other.transform.parent.parent.TryGetComponent<PlayerId>(out PlayerId playerId) && playerId.IsItMyPlayer())
            {
                PlayerHp playerHp = other.GetComponent<PlayerHp>();
                ActivateCheckPoint(playerHp);
            }
        }
    }
}
