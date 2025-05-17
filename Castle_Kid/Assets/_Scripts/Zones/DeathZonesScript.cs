using _Scripts.GameManager;
using _Scripts.Health;
using _Scripts.Projectiles;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace _Scripts.Zones
{
    public class DeathZonesScript : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent<UnitHp>(out UnitHp otherHp))
            {
                otherHp.Die();
                
                if (GM.IsPlayer(other))
                    GM.ProjM.DestroyFollowProjectileServerRpc();
            }
            else if (other.gameObject.TryGetComponent<UnitLocalHp>(out UnitLocalHp otherLocalHp))
            {
                if (GM.IsProjectile(other) && other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
                {
                     if (projectile.Proj.AttackType is ProjectileAttackTypes.OnSender or ProjectileAttackTypes.AroundSender)
                         return;
                }
                
                otherLocalHp.Die();
            }
        }
    }
}
