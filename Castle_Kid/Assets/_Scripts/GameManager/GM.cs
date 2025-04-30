using System;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.GameManager
{
    public class GM : MonoBehaviour
    {
        public static readonly string PlayerTag = "Player";
        public static readonly string PlayerAttackTag = "PlayerAttack";
        public static readonly string PlayerProjectileTag = "PlayerProjectile";
        public static readonly string EnemyTag = "Enemy";
        public static readonly string EnemyAttackTag = "EnemyAttack";
        public static readonly string EnemyProjectileTag = "EnemyProjectile";
        public static readonly int GroundLayerId = 3;

        public static ProjectileManager ProjM;
        
        public void Awake()
        {
            if (ProjM == null)
            {
                ProjM = gameObject.AddComponent<ProjectileManager>();
            }
        }

        public delegate bool FilterType(Collider2D item);

        public static readonly FilterType IsTargetForPlayer = DestroyableElementFromEnemy;
        private static bool DestroyableElementFromEnemy(Collider2D item)
        {
            return item.CompareTag(EnemyTag) ||
                   item.CompareTag(EnemyProjectileTag);
        }

        public static readonly FilterType IsTargetForEnemy = DestroyableElementFromPlayer;
        private static bool DestroyableElementFromPlayer(Collider2D item)
        {
            return item.CompareTag(PlayerTag) ||
                   item.CompareTag(PlayerProjectileTag);
        }

        public static readonly FilterType DestroyableProjectile = ProjectileDestroyableByPlayer;
        private static bool ProjectileDestroyableByPlayer(Collider2D item)
        {
            return item.CompareTag(PlayerAttackTag) ||
                   item.gameObject.layer == GroundLayerId;
        }
        
        public static readonly FilterType NotDestroyableProjectile = ProjectileNotDestroyableByPlayer;
        private static bool ProjectileNotDestroyableByPlayer(Collider2D item)
        {
            return item.gameObject.layer == GroundLayerId;
        }
        
        public static readonly FilterType DestroyableProjectileCrossWalls = ProjectileDestroyableByPlayerCrossWalls;
        private static bool ProjectileDestroyableByPlayerCrossWalls(Collider2D item)
            // by intangible, I mean that I can go thought walls
        {
            return item.CompareTag(PlayerAttackTag);
        }


        public static float GetAngleFromVectorFloat(Vector3 dir) 
        // Don't ask me https://www.youtube.com/watch?v=Nke5JKPiQTw (8:45)
        // basically it takes a direction and return this direction as an euler angle to rotate our item
        {
            dir = dir.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360f;

            return angle;
        }
    }
}
