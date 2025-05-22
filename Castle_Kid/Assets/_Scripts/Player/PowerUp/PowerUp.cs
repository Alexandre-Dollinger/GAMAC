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
    public struct PowerUpStruct : INetworkSerializable
    {
        public ProjectileStruct ProjStruct;
        public ProjectilePrefabs ProjPrefab;
        public float Cooldown;
        public float CurrentTime;
        public bool Enable;

        public PowerUpStruct(ProjectileStruct projStruct, ProjectilePrefabs projPrefab, float cooldown, bool enable = true)
        {
            ProjStruct = projStruct;
            ProjPrefab = projPrefab;
            Cooldown = cooldown;
            CurrentTime = Cooldown;

            Enable = enable;
        }

        public bool ReadyToFire()
        {
            return CurrentTime <= 0;
        }

        public void Fire(Vector3 casterPos, Vector3 direction, string projTag, int senderId)
        {
            CurrentTime = Cooldown;
            ProjStruct.UpdatePosAndDir(casterPos, direction, ProjStruct.Offset);
            GM.ProjM.CreateProjectileManager(ProjStruct, ProjPrefab, projTag, senderId);
        }

        public void CountTimer(float deltaTime)
        {
            if (CurrentTime > 0)
                CurrentTime -= deltaTime;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ProjStruct);
            serializer.SerializeValue(ref Cooldown);
            serializer.SerializeValue(ref CurrentTime);
        }
    }
    
    public class PowerUp : NetworkBehaviour
    {
        private Camera _plCamera;
        
        public PowerUpStruct PowerUp1;
        public PowerUpStruct PowerUp2;
        public PowerUpStruct PowerUp3;
        
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
            
            SetBasicProjectiles();
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

        private void UpdateTimer()
        {
            float deltaTime = Time.deltaTime;

            if (PowerUp1.Enable && !PowerUp1.ReadyToFire())
                PowerUp1.CountTimer(deltaTime);
            if (PowerUp2.Enable && !PowerUp2.ReadyToFire())
                PowerUp2.CountTimer(deltaTime);
            if (PowerUp3.Enable && !PowerUp3.ReadyToFire())
                PowerUp3.CountTimer(deltaTime);
        }

        public void Update()
        {
            UpdateTimer();

            FireManager();
        }
        
        private void FireManager()
        {
                        
            if (InputManager.PowerUp1WasPressed && PowerUp1.Enable && PowerUp1.ReadyToFire())
                PowerUp1.Fire(transform.position, GetMouseDirection(), GM.PlayerProjectileTag, (int)OwnerClientId);
            //FirePowerUp1(GetMouseDirection());
            
            if (InputManager.PowerUp2WasPressed && PowerUp2.Enable && PowerUp2.ReadyToFire())
                PowerUp2.Fire(transform.position, GetMouseDirection(), 
                    GM.PlayerProjectileTag, (int)OwnerClientId);
            //FirePowerUp2(GetMouseDirection());

            if (InputManager.PowerUp3WasPressed && PowerUp3.Enable && PowerUp3.ReadyToFire())
                PowerUp3.Fire(transform.position, GetMouseDirection(), 
                    GM.PlayerProjectileTag, (int)OwnerClientId);
            //FirePowerUp3(GetMouseDirection());

            if (InputManager.AimController != Vector2.zero)
            {
                if (InputManager.PowerUp1ControllerWasPressed && PowerUp1.Enable && PowerUp1.ReadyToFire())
                    PowerUp1.Fire(transform.position,InputManager.AimController.normalized, 
                        GM.PlayerProjectileTag, (int)OwnerClientId);
                
                if (InputManager.PowerUp2ControllerWasPressed && PowerUp2.Enable && PowerUp2.ReadyToFire())
                    PowerUp2.Fire(transform.position, InputManager.AimController.normalized, 
                        GM.PlayerProjectileTag, (int)OwnerClientId);
                
                if (InputManager.PowerUp3ControllerWasPressed && PowerUp3.Enable && PowerUp3.ReadyToFire())
                    PowerUp3.Fire(transform.position, InputManager.AimController.normalized, 
                        GM.PlayerProjectileTag, (int)OwnerClientId);
            } 
        }

        private void SetBasicProjectiles()
        {
            PowerUp1 = new PowerUpStruct(GM.GetBasicLinearProjectileStruct(Vector3.zero, Vector3.zero), 
                ProjectilePrefabs.MagicArrowCone, 1f);

            PowerUp2 = new PowerUpStruct(GM.GetBasicAroundSenderProjectileStruct(Vector3.zero, Vector3.zero),
                ProjectilePrefabs.BlackHoleCone, 5f);

            PowerUp2.ProjStruct.Offset = 100f;
            
            ProjectileStruct healingProj =
                GM.GetBasicLinearProjectileStruct(Vector3.zero, Vector3.zero);

            healingProj.InitHealing(20);
            healingProj.CanBeDestroyedBySelf = false;
            healingProj.CanBeDestroyedByPlayer = false;

            PowerUp3 = new PowerUpStruct(healingProj, ProjectilePrefabs.SparkCone, 3f);
        }

        /*private void FirePowerUp1(Vector3 direction)
        {
            /*ProjectileStruct onSenderProj = GM.GetBasicOnSenderProjectileStruct(transform.position);
                GM.ProjM.CreateProjectileManager(onSenderProj, ProjectilePrefabs.SparkCircle, GM.PlayerProjectileTag, (int)OwnerClientId);#1#
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
            GM.ProjM.CreateProjectileManager(trackingProj, ProjectilePrefabs.SparkCone, GM.PlayerProjectileTag, (int)OwnerClientId);#1#
        }

        private void FirePowerUp3(Vector3 direction)
        {
            //ProjectileStruct onSenderProj = GM.GetBasicOnSenderProjectileStruct(transform.position);
            // GM.ProjM.CreateProjectileManager(onSenderProj, ProjectilePrefabs.ShieldBlueCircle, GM.PlayerProjectileTag, (int)OwnerClientId);
            /*ProjectileStruct trackingProjAccelerating =
                GM.GetBasicTrackingAcceleratingProjectileStruct(transform.position, GetMouseDirection());#1#
                
            ProjectileStruct linearProj =
                GM.GetBasicLinearProjectileStruct(transform.position, direction);

            linearProj.InitHealing(20);
            linearProj.CanBeDestroyedBySelf = false;
            linearProj.CanBeDestroyedByPlayer = false;

            //GM.ProjM.CreateProjectileServerRpc(linearProj, ProjectilePrefabType.Spark, GM.PlayerProjectileTag);
            GM.ProjM.CreateProjectileManager(linearProj, ProjectilePrefabs.SparkCone, GM.PlayerProjectileTag, (int)OwnerClientId);
        }*/
    }
}
