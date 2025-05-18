using System;
using System.Collections;
using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

namespace _Scripts.Player.PowerUp
{
    public class PowerUp : NetworkBehaviour
    {
        [Header("References")] public PowerUpStats powerStats;

        [SerializeField] private GameObject toSpawn;

        private Camera _plCamera;

        private Random _random = new Random(0);

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
            return _plCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        private ProjectilePrefabs GetRandomProjectilePrefabs()
        {
            return (ProjectilePrefabs)_random.Next(0, 14);
        }
        

        public void Update()
        {
            if (InputManager.PowerUp1WasPressed)
                FirePowerUp1(GetMouseDirection());
            
            if (InputManager.PowerUp2WasPressed)
                FirePowerUp2(GetMouseDirection());

            if (InputManager.PowerUp3WasPressed)
                FirePowerUp3(GetMouseDirection());

            if (InputManager.AimController != Vector2.zero)
            {
                if (InputManager.PowerUp1ControllerIsPressed)
                    FirePowerUp1(InputManager.AimController.normalized);
                
                if (InputManager.PowerUp2ControllerIsPressed)
                    FirePowerUp2(InputManager.AimController.normalized);
                
                if (InputManager.PowerUp3ControllerIsPressed)
                    FirePowerUp3(InputManager.AimController.normalized);
            } 
        }

        private void FirePowerUp1(Vector3 direction)
        {
            /*ProjectileStruct onSenderProj = GM.GetBasicOnSenderProjectileStruct(transform.position);
                GM.ProjM.CreateProjectileManager(onSenderProj, ProjectilePrefabs.SparkCircle, GM.PlayerProjectileTag, (int)OwnerClientId);*/
            ProjectileStruct linearProj =
                GM.GetBasicLinearProjectileStruct(transform.position, direction);

            GM.ProjM.CreateProjectileManager(linearProj, ProjectilePrefabs.MagicArrowCone, GM.PlayerProjectileTag, (int)OwnerClientId);
        }

        private void FirePowerUp2(Vector3 direction)
        {
            ProjectileStruct aroundSenderProj = GM.GetBasicAroundSenderProjectileStruct(transform.position, direction);
            GM.ProjM.CreateProjectileManager(aroundSenderProj, ProjectilePrefabs.BlackHoleCone, GM.PlayerProjectileTag, (int)OwnerClientId, Vector3.Distance(transform.position, GetMousePos()));
            /*ProjectileStruct trackingProj =
                GM.GetBasicTrackingFixedSpeedProjectileStruct(transform.position, GetMouseDirection());
            GM.ProjM.CreateProjectileManager(trackingProj, ProjectilePrefabs.SparkCone, GM.PlayerProjectileTag, (int)OwnerClientId);*/
        }

        private void FirePowerUp3(Vector3 direction)
        {
            //ProjectileStruct onSenderProj = GM.GetBasicOnSenderProjectileStruct(transform.position);
            // GM.ProjM.CreateProjectileManager(onSenderProj, ProjectilePrefabs.ShieldBlueCircle, GM.PlayerProjectileTag, (int)OwnerClientId);
            /*ProjectileStruct trackingProjAccelerating =
                GM.GetBasicTrackingAcceleratingProjectileStruct(transform.position, GetMouseDirection());*/
                
            ProjectileStruct linearProj =
                GM.GetBasicLinearProjectileStruct(transform.position, direction);

            linearProj.InitHealing(20);
            linearProj.CanBeDestroyedBySelf = false;
            linearProj.CanBeDestroyedByPlayer = false;

            //GM.ProjM.CreateProjectileServerRpc(linearProj, ProjectilePrefabType.Spark, GM.PlayerProjectileTag);
            GM.ProjM.CreateProjectileManager(linearProj, ProjectilePrefabs.SparkCone, GM.PlayerProjectileTag, (int)OwnerClientId);
        }
    }
}
