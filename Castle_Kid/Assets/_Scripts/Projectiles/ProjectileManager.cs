using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

        //public List<GameObject> prefabList;
        
        [ServerRpc]
        public void CreateProjectileServerRpc(ProjectileType projType, string projTag, 
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            /*Transform transformProj = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
            GetProjectilePrefab(projType),position: spawnPos,rotation: Quaternion.identity).transform; 
            // equivalent with : Instantiate(GetProjectilePrefab(projType),spawnPos, Quaternion.identity);  but for network
            
            
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                GetProjectilePrefab(projType),position: spawnPos,rotation: Quaternion.identity);
            //transformProj.gameObject.GetComponent<NetworkObject>().Spawn();
            Transform transformProj = networkObject.transform;
            */

            //InstantiateServerRpc(GetProjectilePrefab(projType), spawnPos, Quaternion.identity);
            
            CreateProjectileClientRpc(projType, projTag, startPos, shootDir, offset, scale);
            
            /*_recentProjectile.tag = projTag;
            
            Projectile projectile = _recentProjectile.GetComponent<Projectile>();
            
            projectile.BasicInit(spawnPos, shootDir, scale);
            
            return projectile;*/
        }

        public Projectile CreateProjectile(ProjectileType projType, string projTag,
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            Vector3 spawnPos = startPos + (shootDir * offset);

            Transform transformProj =
                Instantiate(GetProjectilePrefab(projType), spawnPos, Quaternion.identity);

            transformProj.tag = projTag;

            Projectile projectile = transformProj.GetComponent<Projectile>();
            
            projectile.BasicInit(spawnPos, shootDir, scale);

            return projectile;
        }

        [ClientRpc]
        public void CreateProjectileClientRpc(ProjectileType projType, string projTag,
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            Vector3 spawnPos = startPos + (shootDir * offset);

            Transform transformProj = Instantiate(GetProjectilePrefab(projType),spawnPos, Quaternion.identity).transform;

            transformProj.tag = projTag;

            Projectile projectile = transformProj.GetComponent<Projectile>();
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