using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using _Scripts.Enemy;
using _Scripts.GameManager;

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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Collision");
            if (GM.IsPlayer(collision.collider))
            {
                PlayerHp playerHp = collision.collider.gameObject.GetComponent<PlayerHp>();
                playerHp.TakeDamage(10);
            }
        }


        public override void Die()
        {
            if (IsServer)
                Destroy(transform.parent.parent.gameObject);
        }
    }
}