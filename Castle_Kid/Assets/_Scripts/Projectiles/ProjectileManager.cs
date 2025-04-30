using System;
using _Scripts.GameManager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Scripts.Projectiles
{
    public enum ProjectilesType
    {
        Spark,
    }
    
    public class ProjectileManager : MonoBehaviour
    {
        public Transform sparkPrefab;

        public Projectile CreateProjectile(ProjectilesType projType, string projTag, 
            int damage, float speed, Vector3 startPos, Vector3 shootDir,
            GM.FilterType filterTarget, bool crossWalls = false, bool canDie = true, float scale = 1, 
            bool tracking = false, Vector3? targetPos = null)
        {
            Transform transformProj = Instantiate(GetProjectilePrefab(projType), startPos, Quaternion.identity);

            transformProj.tag = projTag;

            GM.FilterType filterDestroy = crossWalls ? canDie ? GM.DestroyableProjectileCrossWalls : null :
                canDie ? GM.DestroyableProjectile : GM.NotDestroyableProjectile;
            
            Projectile projectile = transformProj.GetComponent<Projectile>();
            projectile.InitProjectile(damage, speed, startPos, shootDir, filterTarget, filterDestroy, scale, tracking, targetPos);
            
            return projectile;
        }
        
        private Transform GetProjectilePrefab(ProjectilesType projType)
        {
            switch (projType)
            {
                case ProjectilesType.Spark:
                    return sparkPrefab;
            }

            throw new NotImplementedException($"Projectile not known : {projType}");
        }
    }
}
