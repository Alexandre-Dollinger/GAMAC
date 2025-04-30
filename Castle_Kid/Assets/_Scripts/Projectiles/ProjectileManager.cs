using System;
using _Scripts.GameManager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Scripts.Projectiles
{
    public enum ProjectileType
    {
        Spark,
    }

    public enum ProjectileAttackType
    {
        Linear,
        Tracking,
        Parabola,
    }
    
    public class ProjectileManager : MonoBehaviour
    {
        public Transform sparkPrefab;

        public Projectile CreateProjectile(ProjectileType projType, string projTag, 
            Vector3 startPos, Vector3 shootDir, float offset = 15, float scale = 1f)
        {
            Transform transformProj = Instantiate(GetProjectilePrefab(projType), startPos, Quaternion.identity);

            transformProj.tag = projTag;
            
            Projectile projectile = transformProj.GetComponent<Projectile>();
            
            projectile.BasicInit(startPos, shootDir, offset, scale);
            
            return projectile;
        }
        
        private Transform GetProjectilePrefab(ProjectileType projType)
        {
            switch (projType)
            {
                case ProjectileType.Spark:
                    return sparkPrefab;
            }

            throw new NotImplementedException($"Projectile not known : {projType}");
        }
    }
}
