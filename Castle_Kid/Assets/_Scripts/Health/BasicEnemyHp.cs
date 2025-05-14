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
            if (IsServer)
                Destroy(transform.parent.parent.gameObject);
        }
    }
}