using _Scripts.Projectiles;
using _Scripts.Enemy;
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
        public static int CurrentPlayerSortingLayerId;
        public static int BehindProjectileSortingLayer;
        
        public static bool GameStarted = false;

        public static ProjectileManager ProjM;
        public static EnemyManager EnemyM;

        public static PlayerTracking playerTracking;
        public static DialogueManager dialogueManager;
        
        public void Awake()
        {
            if (ProjM == null)
                ProjM = GameObject.Find("PROJECTILE_MANAGER").GetComponent<ProjectileManager>();

            if (EnemyM == null)
                EnemyM = GameObject.Find("ENEMY_MANAGER").GetComponent<EnemyManager>();

            if (playerTracking == null)
                playerTracking = GameObject.Find("ENEMY_MANAGER").GetComponent<PlayerTracking>();
            
            if (dialogueManager == null)
                dialogueManager = GameObject.Find("DIALOGUE_MANAGER").GetComponent<DialogueManager>();

            CurrentPlayerSortingLayerId = SortingLayer.NameToID("CurrentPayer");
            BehindProjectileSortingLayer = SortingLayer.NameToID("Behind_Projectiles");
        }

        #region FilterType
        public delegate bool FilterType(Collider2D item);

        #region TargetFor
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
        #endregion

        public static readonly FilterType CrossedWall = TriedToCrossWalls;
        private static bool TriedToCrossWalls(Collider2D item)
        {
            return item.gameObject.layer == GroundLayerId;
        }

        public static readonly FilterType IsPlayer = IsItAPlayer;
        private static bool IsItAPlayer(Collider2D item)
        {
            return item.CompareTag(PlayerTag);
        }
        
        public static readonly FilterType IsEnemy = IsItAEnemy;
        private static bool IsItAEnemy(Collider2D item)
        {
            return item.CompareTag(EnemyTag);
        }
        
        public static readonly FilterType IsPlayerProjectile = IsItAPlayerProjectile;
        private static bool IsItAPlayerProjectile(Collider2D item)
        {
            return item.CompareTag(PlayerProjectileTag);
        }
        
        public static readonly FilterType IsEnemyProjectile = IsItAEnemyProjectile;
        private static bool IsItAEnemyProjectile(Collider2D item)
        {
            return item.CompareTag(EnemyProjectileTag);
        }

        public static readonly FilterType IsProjectile = IsItAProjectile;
        private static bool IsItAProjectile(Collider2D item)
        {
            return IsPlayerProjectile(item) || IsEnemyProjectile(item);
        }
        #endregion
        
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

        #region GetProjectile
        public static ProjectileStruct GetBasicLinearProjectileStruct(Vector3 spawnPos, Vector3 direction)
        {
            ProjectileStruct linearProj = new ProjectileStruct(spawnPos, direction);
            linearProj.InitSpeed(100);
            linearProj.InitDestroyCondition();
            linearProj.InitTargeting(true, true, true, true);
            linearProj.InitAttackLinear(50);

            return linearProj;
        }
        
        public static ProjectileStruct GetBasicTrackingFixedSpeedProjectileStruct(Vector3 spawnPos, Vector3 direction)
        {
            ProjectileStruct trackingProj = new ProjectileStruct(spawnPos, direction);
            trackingProj.InitSpeed();
            trackingProj.InitDestroyCondition(true, 50, true);
            trackingProj.InitTargeting(true, true, true, true);
            trackingProj.InitAttackTracking(50, 200);

            return trackingProj;
        }
        
        public static ProjectileStruct GetBasicTrackingAcceleratingProjectileStruct(Vector3 spawnPos, Vector3 direction)
        {
            ProjectileStruct trackingProj = new ProjectileStruct(spawnPos, direction);
            trackingProj.InitSpeed(50,10 ,200);
            trackingProj.InitDestroyCondition(true, 50, true);
            trackingProj.InitTargeting(true, true, true, true);
            trackingProj.InitAttackTracking(50, 200);

            return trackingProj;
        }
        
        public static ProjectileStruct GetBasicOnSenderProjectileStruct(Vector3 spawnPos)
        {
            ProjectileStruct onSenderProj = new ProjectileStruct(spawnPos, Vector3.zero);
            onSenderProj.InitDestroyCondition(true, 50, true, false);
            onSenderProj.InitTargeting(true, true, false, true, false);
            onSenderProj.InitAttackOnSender(50, SenderTags.Player);
            onSenderProj.BecomeBehindPlayerProjectile();

            return onSenderProj;
        }
        
        public static ProjectileStruct GetBasicAroundSenderProjectileStruct(Vector3 spawnPos, Vector3 direction)
        {
            ProjectileStruct aroundSenderProj = new ProjectileStruct(spawnPos, direction, scale:1.5f);
            aroundSenderProj.InitDestroyCondition(true, 50, true, true);
            aroundSenderProj.InitTargeting(true, true, false, true, false);
            aroundSenderProj.InitAttackAroundSender(50, SenderTags.Player, 180f, true, 45 ,false);  
            //aroundSenderProj.BecomeBehindPlayerProjectile();

            return aroundSenderProj;
        }
        
        public static ProjectileStruct GetBasicFixProjectileStruct(Vector3 spawnPos)
        {
            ProjectileStruct fixProj = new ProjectileStruct(spawnPos, Vector3.zero);
            fixProj.InitDestroyCondition(true, 50, true);
            fixProj.InitTargeting(true, true, true, true);
            fixProj.InitAttackFix(50, 180f, false);

            return fixProj;
        }

        public static ProjectileStruct GetLinearProjectileStruct()
        {
            ProjectileStruct linearProj = new ProjectileStruct(Vector3.zero, Vector3.zero);
            linearProj.InitSpeed(100);
            linearProj.InitDestroyCondition();
            linearProj.InitTargeting(true, true, true, true);
            linearProj.InitAttackLinear(50);

            return linearProj;
        }

        public static ProjectileStruct GetTrackingProjectileStruct()
        {
            ProjectileStruct trackingProj = new ProjectileStruct(Vector3.zero, Vector3.zero);
            trackingProj.InitSpeed();
            trackingProj.InitDestroyCondition(true, 50, true);
            trackingProj.InitTargeting(true, true, true, true);
            trackingProj.InitAttackTracking(50, 200);

            return trackingProj;
        }

        public static ProjectileStruct GetOnSenderProjectileStruct()
        {
            ProjectileStruct onSenderProj = new ProjectileStruct(Vector3.zero, Vector3.zero);
            onSenderProj.InitDestroyCondition(true, 50, true, false);
            onSenderProj.InitTargeting(true, true, false, true, false);
            onSenderProj.InitAttackOnSender(50, SenderTags.Player);
            onSenderProj.BecomeBehindPlayerProjectile();

            return onSenderProj;
        }

        public static ProjectileStruct GetAroundSenderProjectileStruct()
        {
            ProjectileStruct aroundSenderProj = new ProjectileStruct(Vector3.zero, Vector3.zero, 100f, 1.5f);
            aroundSenderProj.InitDestroyCondition(true, 50, true, true);
            aroundSenderProj.InitTargeting(true, true, false, true, true);
            aroundSenderProj.InitAttackAroundSender(50, SenderTags.Player, 0f, true, 45 ,false);  

            return aroundSenderProj;
        }

        public static ProjectileStruct GetFixProjectileStruct()
        {
            ProjectileStruct fixProj = new ProjectileStruct(Vector3.zero, Vector3.zero, scale: 0.7f, offset: 100f);
            fixProj.InitDestroyCondition(true, 50, true);
            fixProj.InitTargeting(true, true, true, true);
            fixProj.InitAttackFix(50, 0f, false);

            return fixProj;
        }
        
        #endregion
    }
}
