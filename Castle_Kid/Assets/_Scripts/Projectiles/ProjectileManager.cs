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
        public GameObject sparkPrefab;

        //public List<GameObject> prefabList;
        
        /*[ServerRpc]
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
            #1#
            /*_recentProjectile.tag = projTag;

            Projectile projectile = _recentProjectile.GetComponent<Projectile>();

            projectile.BasicInit(spawnPos, shootDir, scale);

            return projectile;#1#
            
            Vector3 spawnPos = startPos + (shootDir * offset);

            Transform transformProj = Instantiate(GetProjectilePrefab(projType),spawnPos, Quaternion.identity).transform;
            
            transformProj.gameObject.GetComponent<NetworkObject>().Spawn();

            transformProj.tag = projTag;

            Projectile projectile = transformProj.GetComponent<Projectile>();
            
            projectile.BasicInitServerRpc(spawnPos, shootDir, scale);
            
            CreateProjectileClientRpc(projType, projTag, startPos, shootDir, offset, scale);
        }*/
        
        [ServerRpc]
        public void CreateProjectileServerRPC()
        {
            GameObject gameObjectProj = Instantiate(sparkPrefab, new Vector3(0,0,0), Quaternion.identity);
            
            gameObjectProj.GetComponent<NetworkObject>().Spawn();
        }
        
        [ClientRpc]
        public void CreateProjectileClientRpc(ProjectileType projType, string projTag,
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            Vector3 spawnPos = startPos + (shootDir * offset);

            Transform transformProj = Instantiate(GetProjectilePrefab(projType),spawnPos, Quaternion.identity).transform;

            transformProj.gameObject.GetComponent<NetworkObject>().Spawn();
            
            transformProj.tag = projTag;

            Projectile projectile = transformProj.GetComponent<Projectile>();
            
            //projectile.BasicInitClientRpc(spawnPos, shootDir, scale);
        }

        /*public Projectile CreateProjectile(ProjectileType projType, string projTag,
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            Vector3 spawnPos = startPos + (shootDir * offset);

            Transform transformProj =
                Instantiate(GetProjectilePrefab(projType), spawnPos, Quaternion.identity);

            transformProj.tag = projTag;

            Projectile projectile = transformProj.GetComponent<Projectile>();
            
            projectile.BasicInit(spawnPos, shootDir, scale);

            return projectile;
        }*/
        
        private GameObject GetProjectilePrefab(ProjectileType projType)
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