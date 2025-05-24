using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using _Scripts.Enemy;
using _Scripts.GameManager;
using _Scripts.Player.PowerUp;
using _Scripts.Projectiles;
using _Scripts.Map;

namespace _Scripts.Health
{
    public class BasicEnemyHp : UnitHp
    {
        private float collisionDmgTime = 0.5f;
        private float collisionDmgCooldown = 0f;

        private BasicEnemy basicEnemy;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                //To get the enemy this script belongs to
                basicEnemy = gameObject.transform.parent.parent.GetComponent<BasicEnemy>();

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
            CountTimers();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!GM.IsEnemy(collision.collider))
            {
                if (collision.collider.gameObject.TryGetComponent(out IUnitHp otherHp) && collisionDmgCooldown <= 0)
                {
                    if (otherHp.IsNetwork)
                    {
                        collisionDmgCooldown = collisionDmgTime;
                        
                        if (IsServer)
                            otherHp.TakeDamage(10);
                    }
                    else
                    {
                        collisionDmgCooldown = collisionDmgTime;
                        otherHp.TakeDamage(10);
                    }
                }
            }
        }

        public override void Die()
        {
            if (IsServer)
            {
                basicEnemy.powerUp.ProjStruct.InitTargeting(true, true, false, true, false);
                SpawnPowerUpServerRpc(basicEnemy.powerUp);
                Destroy(transform.parent.parent.gameObject);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPowerUpServerRpc(PowerUpStruct powerUpStruct)
        {
            GameObject drop = Instantiate(GM.EnemyM.PowerUpCollectablePrefab, transform.position, Quaternion.identity);
            PowerUpCollectableScript collectableScript = drop.GetComponent<PowerUpCollectableScript>();
            collectableScript.SetPowerUpStructServerRpc(powerUpStruct);
            drop.GetComponent<NetworkObject>().Spawn();    
        }

        private void CountTimers()
        {
            float deltaTime = Time.deltaTime;

            if (collisionDmgCooldown > 0)
            {
                collisionDmgCooldown -= deltaTime;
            }
        }
    }
}