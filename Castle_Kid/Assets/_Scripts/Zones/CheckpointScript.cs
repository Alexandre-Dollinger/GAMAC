using System;
using _Scripts.GameManager;
using _Scripts.Health;
using UnityEngine;

namespace _Scripts.Zones
{
    public class CheckpointScript : MonoBehaviour
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
            playerHp.GainFullLife();
            
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
            if (other.CompareTag(GM.PlayerTag))
            {
                PlayerHp playerHp = other.GetComponent<PlayerHp>();
                ActivateCheckPoint(playerHp);
            }
        }
    }
}
