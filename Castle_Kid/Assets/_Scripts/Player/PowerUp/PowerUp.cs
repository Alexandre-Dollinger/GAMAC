using System;
using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using _Scripts.UI_scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
            CurrentTime = 0f;
            
            Enable = enable;
        }

        public bool ReadyToFire()
        {
            return Enable && CurrentTime <= 0;
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
        
        public Sprite GetPrefabSprite()
        {
            return GM.ProjM.GetProjectilePrefab(ProjPrefab).GetComponent<SpriteRenderer>().sprite;
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

        [SerializeField] private CooldownUI pow1Cooldown;
        [SerializeField] private CooldownUI pow2Cooldown;
        [SerializeField] private CooldownUI pow3Cooldown;
        
        private int _lastInited = 3;
        private int NextToInit
        {
            get
            {
                _lastInited = _lastInited % 3 + 1;
                return _lastInited;
            }
        }
        
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
        
        public void Update()
        {
            UpdateTimer();
            
            UpdateCooldown();

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
            
            UpdateSpriteUI(1);
            UpdateSpriteUI(2);
            UpdateSpriteUI(3);
        }

        private void UpdateSpriteUI(int numPowerUp)
        {
            switch (numPowerUp)
            {
                case 1:
                    pow1Cooldown.UpdateSprite(PowerUp1.GetPrefabSprite());
                    break;
                case 2:
                    pow2Cooldown.UpdateSprite(PowerUp2.GetPrefabSprite());
                    break;
                case 3:
                    pow3Cooldown.UpdateSprite(PowerUp3.GetPrefabSprite());
                    break;
                default:
                    throw new ArgumentException("What PowerUp num is that ??? : " + numPowerUp);
            }
        }

        public void UpdatePowerUp(PowerUpStruct powerUpStruct)
        {
            int nextToInit = NextToInit;
            
            switch (nextToInit)
            {
                case 1:
                    PowerUp1 = powerUpStruct;
                    break;
                case 2:
                    PowerUp2 = powerUpStruct;
                    break;
                case 3:
                    PowerUp3 = powerUpStruct;
                    break;
            }
            
            UpdateSpriteUI(nextToInit);
        }

        private void UpdateCooldown()
        {
            if (PowerUp1.Enable)
                pow1Cooldown.CooldownManagement(PowerUp1.CurrentTime, PowerUp1.Cooldown);
            if (PowerUp2.Enable)
                pow2Cooldown.CooldownManagement(PowerUp2.CurrentTime, PowerUp2.Cooldown);
            if (PowerUp3.Enable)
                pow3Cooldown.CooldownManagement(PowerUp3.CurrentTime, PowerUp3.Cooldown);
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
    }
}