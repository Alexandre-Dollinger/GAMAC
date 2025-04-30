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
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            Vector3 spawnPos = startPos + (shootDir * offset);
            Transform transformProj = Instantiate(GetProjectilePrefab(projType), 
                spawnPos, Quaternion.identity);

            transformProj.tag = projTag;
            
            Projectile projectile = transformProj.GetComponent<Projectile>();
            
            projectile.BasicInit(spawnPos, shootDir, scale);
            
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