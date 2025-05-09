using System;
using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player.PowerUp
{
    public class PowerUp : MonoBehaviour
    {
        [Header("References")]
        public PowerUpStats powerStats;

        private Camera _plCamera;
        
        /*public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
        }*/
        
        public void Awake()
        {
            _plCamera = GetComponentInChildren<Camera>();
        }

        private Vector3 GetMouseDirection()
        {
            // just accept it or go look here : https://discussions.unity.com/t/point-towards-mouse-position/876845/4
            Vector2 mousePos = _plCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 direction = (mousePos - (Vector2)transform.position).normalized;

            return direction;
        }

        public void Update()
        {
            if (InputManager.PowerUp1WasReleased)
            {
                //GM.ProjM.CreateProjectileServerRpc(ProjectileType.Spark, GM.PlayerProjectileTag, transform.position, GetMouseDirection());
                SpawnLinearProjectile();
            }
            
            if (InputManager.PowerUp2WasReleased)
            {
                SpawnTrackingFixedSpeedProjectile();
            }
            
            if (InputManager.PowerUp3WasReleased)
            {
                SpawnTrackingAcceleratingProjectile();
            }
        }
        
        

        private void SpawnLinearProjectile()
        {
            GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                transform.position, GetMouseDirection());
                
            GM.ProjM.GetLastProjectile().InitDestroyCondition(GM.IsPlayer);
            GM.ProjM.GetLastProjectile().InitSpeed(100);
            GM.ProjM.GetLastProjectile().InitAttackLinear(50);
        }

        private void SpawnTrackingFixedSpeedProjectile()
        {
            GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                transform.position, GetMouseDirection());
                
            GM.ProjM.GetLastProjectile().InitDestroyCondition(GM.IsPlayer, true, 50, true);
            GM.ProjM.GetLastProjectile().InitSpeed();
            GM.ProjM.GetLastProjectile().InitAttackTracking(50,200, transform);
        }

        private void SpawnTrackingAcceleratingProjectile()
        {
            GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                transform.position, GetMouseDirection());
                
            GM.ProjM.GetLastProjectile().InitDestroyCondition(GM.IsPlayer, true, 50, true);
            GM.ProjM.GetLastProjectile().InitSpeed(10, 50, 1000);
            GM.ProjM.GetLastProjectile().InitAttackTracking(50,200, transform);
        }
    }
}
