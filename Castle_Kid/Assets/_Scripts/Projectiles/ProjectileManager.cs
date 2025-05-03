using System;
using _Scripts.GameManager;
using Unity.Netcode;
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
    
    public class ProjectileManager : NetworkBehaviour
    {
        public Transform sparkPrefab;
        
        public Projectile CreateProjectile(ProjectileType projType, string projTag, 
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            Vector3 spawnPos = startPos + (shootDir * offset);
            
            /*Transform transformProj = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
            GetProjectilePrefab(projType),position: spawnPos,rotation: Quaternion.identity).transform; 
            // equivalent with : Instantiate(GetProjectilePrefab(projType),spawnPos, Quaternion.identity));  but for network
            
            
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                GetProjectilePrefab(projType),position: spawnPos,rotation: Quaternion.identity);
            //transformProj.gameObject.GetComponent<NetworkObject>().Spawn();
            Transform transformProj = networkObject.transform;
            */
            
            Transform transformProj = Instantiate(GetProjectilePrefab(projType),spawnPos, Quaternion.identity);
            
            transformProj.gameObject.GetComponent<NetworkObject>().Spawn();
               
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