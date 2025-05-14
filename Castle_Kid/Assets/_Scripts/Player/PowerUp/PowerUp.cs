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
            Vector2 mousePos = GetMousePos();
            Vector3 direction = (mousePos - (Vector2)transform.position).normalized;
            
            return direction;
        }

        private Vector2 GetMousePos()
        {
            return _plCamera.ScreenToWorldPoint( Mouse.current.position.ReadValue());
        }

        public void Update()
        {
            if (InputManager.PowerUp1WasReleased)
            {
                /*ProjectileStruct onSenderProj = GM.GetBasicOnSenderProjectileStruct(transform.position);
                GM.ProjM.CreateProjectileManager(onSenderProj, ProjectilePrefabs.SparkCircle, GM.PlayerProjectileTag, (int)OwnerClientId);*/
                ProjectileStruct linearProj =
                    GM.GetBasicLinearProjectileStruct(transform.position, GetMouseDirection());
                linearProj.InitHealing();
                GM.ProjM.CreateProjectileManager(linearProj, ProjectilePrefabs.SparkCone, GM.PlayerProjectileTag, (int)OwnerClientId);
            }
            
            if (InputManager.PowerUp2WasReleased)
            {
                ProjectileStruct aroundSenderProj = GM.GetBasicAroundSenderProjectileStruct(transform.position, GetMouseDirection());
                aroundSenderProj.RotateSpeed = 0f;
                GM.ProjM.CreateProjectileManager(aroundSenderProj, ProjectilePrefabs.BlackHoleCone, GM.PlayerProjectileTag, (int)OwnerClientId, Vector3.Distance(transform.position, GetMousePos()));
                /*ProjectileStruct trackingProj =
                    GM.GetBasicTrackingFixedSpeedProjectileStruct(transform.position, GetMouseDirection());
                GM.ProjM.CreateProjectileManager(trackingProj, ProjectilePrefabs.SparkCone, GM.PlayerProjectileTag, (int)OwnerClientId);*/
            }
            
            if (InputManager.PowerUp3WasReleased)
            {
                ProjectileStruct onSenderProj = GM.GetBasicOnSenderProjectileStruct(transform.position);
                GM.ProjM.CreateProjectileManager(onSenderProj, ProjectilePrefabs.ShieldBlueCircle, GM.PlayerProjectileTag, (int)OwnerClientId);
                /*ProjectileStruct trackingProjAccelerating =
                    GM.GetBasicTrackingAcceleratingProjectileStruct(transform.position, GetMouseDirection());
                GM.ProjM.CreateProjectileManager(trackingProjAccelerating, ProjectilePrefab.SparkCone, GM.PlayerProjectileTag, (int)OwnerClientId);*/
            }
        }
    }
}
