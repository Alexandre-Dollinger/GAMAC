using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.Player.PowerUp
{
    public class PowerUp : MonoBehaviour
    {
        [Header("References")]
        public PowerUpStats powerStats;

        public void Update()
        {
            if (InputManager.PowerUp1WasReleased)
            {
                Vector3 offsetSpawn = new Vector3(50,0,0);
                Projectile projectile = GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                    transform.position + offsetSpawn, Vector3.right);
                
                projectile.InitDestroyCondition(GM.IsPlayer);
                projectile.InitSpeed(100);
                projectile.InitAttackLinear(50);
            }
            
            if (InputManager.PowerUp2WasReleased)
            {
                Vector3 offsetSpawn = new Vector3(50,0,0);
                Projectile projectile = GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                    transform.position + offsetSpawn, Vector3.right);
                
                projectile.InitDestroyCondition(GM.IsPlayer, true, 50, true);
                projectile.InitSpeed();
                projectile.InitAttackTracking(50,200, transform);
            }
            
            if (InputManager.PowerUp3WasReleased)
            {
                Vector3 offsetSpawn = new Vector3(50,0,0);
                Projectile projectile = GM.ProjM.CreateProjectile(ProjectileType.Spark, GM.PlayerProjectileTag,
                    transform.position + offsetSpawn, Vector3.right);
                
                projectile.InitDestroyCondition(GM.IsPlayer, true, 50, true);
                projectile.InitSpeed(10, 50, 1000);
                projectile.InitAttackTracking(50,200, transform);
            }
        }
    }
}
