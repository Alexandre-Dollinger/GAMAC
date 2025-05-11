using System;
using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player.PowerUp
{
    public class PowerUp : NetworkBehaviour
    {
        [Header("References")]
        public PowerUpStats powerStats;

        [SerializeField] private GameObject toSpawn;

        private Camera _plCamera;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
        }
        
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

        [ServerRpc]
        private void SpawnServerRpc()
        {
            GameObject spawned = Instantiate(toSpawn, transform.position, Quaternion.identity);
            spawned.GetComponent<NetworkObject>().Spawn(true);
        }

        public void Update()
        {
            if (InputManager.PowerUp1WasReleased)
            {
                ProjectileStruct linearProj =
                    GM.GetBasicLinearProjectileStruct(transform.position, GetMouseDirection());
                GM.ProjM.CreateProjectileServerRpc(linearProj, ProjectilePrefabType.Spark, GM.PlayerProjectileTag);
            }
            
            if (InputManager.PowerUp2WasReleased)
            {
                ProjectileStruct trackingProj =
                    GM.GetBasicTrackingFixedSpeedProjectileStruct(transform.position, GetMouseDirection());
                GM.ProjM.CreateProjectileServerRpc(trackingProj, ProjectilePrefabType.Spark, GM.PlayerProjectileTag);
            }
            
            if (InputManager.PowerUp3WasReleased)
            {
                ProjectileStruct trackingProjAccelerating = 
                    GM.GetBasicTrackingAcceleratingProjectileStruct(transform.position, GetMouseDirection());
                GM.ProjM.CreateProjectileServerRpc(trackingProjAccelerating, ProjectilePrefabType.Spark, GM.PlayerProjectileTag);
            }
        }
    }
}
