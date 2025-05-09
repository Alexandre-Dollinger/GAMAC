using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Projectile> projList;

        public Projectile GetLastProjectile()
        {
            if (projList.Count == 0)
                Debug.Log("Cannot your projectile, it hasn't be spawned here !");
            
            return projList.Last();
        }
        
        public void CreateProjectile(ProjectileType projType, string projTag,
            Vector3 startPos, Vector3 shootDir, float offset = 50, float scale = 1f)
        {
            Vector3 spawnPos = startPos + (shootDir * offset);
            
            if (IsOwner)
                CreateProjectileServerRpc(projTag, projType, spawnPos, shootDir, scale);
        }

        [ServerRpc]
        private void CreateProjectileServerRpc(string projTag, ProjectileType projType,
            Vector3 startPos, Vector3 shootDir, float scale)
        {
            CreateProjectileClientRpc(projTag, projType, startPos, shootDir, scale);
        }
        
        [ClientRpc(RequireOwnership = false)]
        private void CreateProjectileClientRpc(string projTag, ProjectileType projType,
            Vector3 startPos, Vector3 shootDir, float scale)
        {
            Transform transformProj =
                Instantiate(GetProjectilePrefab(projType), startPos, Quaternion.identity).transform;

            transformProj.tag = projTag;

            Projectile projectile = transformProj.GetComponent<Projectile>();
            
            projectile.BasicInit(startPos, shootDir, scale);
            
            projList.Add(projectile);
        }
        
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