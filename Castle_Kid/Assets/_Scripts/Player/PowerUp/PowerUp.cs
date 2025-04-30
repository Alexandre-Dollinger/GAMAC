using System;
using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player.PowerUp
{
    public class PowerUp : MonoBehaviour
    {
        [Header("References")]
        public PowerUpStats powerStats;

        private Camera _plCamera;
        
        public void Awake()
        {
            _plCamera = GetComponentInChildren<Camera>();
        }

        private Vector3 GetMouseDirection()
        {
            Vector2 mousePos = _plCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 direction = (mousePos - (Vector2)transform.position).normalized;

            return direction;
        }

        public void Update()
        {
            if (InputManager.PowerUp1WasReleased)
            {
                Projectile projectile = GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                    transform.position, GetMouseDirection());
                
                projectile.InitDestroyCondition(GM.IsPlayer);
                projectile.InitSpeed(100);
                projectile.InitAttackLinear(50);
            }
            
            if (InputManager.PowerUp2WasReleased)
            {
                Projectile projectile = GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                    transform.position, GetMouseDirection());
                
                projectile.InitDestroyCondition(GM.IsPlayer, true, 50, true);
                projectile.InitSpeed();
                projectile.InitAttackTracking(50,200, transform);
            }
            
            if (InputManager.PowerUp3WasReleased)
            {
                Projectile projectile = GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                    transform.position, GetMouseDirection());
                
                projectile.InitDestroyCondition(GM.IsPlayer, true, 50, true);
                projectile.InitSpeed(10, 50, 1000);
                projectile.InitAttackTracking(50,200, transform);
            }
        }
    }
}
