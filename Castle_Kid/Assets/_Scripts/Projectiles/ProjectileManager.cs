using System;
using System.Collections.Generic;
using _Scripts.GameManager;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ProjectileManager : NetworkBehaviour // https://www.youtube.com/watch?v=3yuBOB3VrCk&t=232s
    {   
        public ProjectileScriptableObject ProjSO;
        
        public GameObject sparkPrefab;

        public List<Projectile> projList; // A list only for the owner
 
        [ServerRpc(RequireOwnership = false)]
        public void CreateProjectileServerRpc(ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, float offset = 50, ServerRpcParams serverRpcParams = default)
        {
            projStruct.SpawnPos += projStruct.Direction * offset;
            // To not spawn the projectile in our caster

            GameObject gameObjectProj = Instantiate(GetProjectilePrefab(projPrefabType), 
                projStruct.SpawnPos, Quaternion.identity);
            
            gameObjectProj.GetComponent<NetworkObject>().Spawn(true);
            
            gameObjectProj.tag = projTag;

            Projectile projectile = gameObjectProj.GetComponent<Projectile>();

            if (projTag == GM.PlayerProjectileTag) // if it does not come from a player then we don't care about it's sender Id 
                projStruct.SenderId = (int)serverRpcParams.Receive.SenderClientId;
            
            projectile.InitProjectile(projStruct);
            
            projList.Add(projectile);
        }
        
        private GameObject GetProjectilePrefab(ProjectilePrefabType projType)
        {
            switch (projType)
            {
                case ProjectilePrefabType.Spark:
                    return sparkPrefab;
            }

            throw new NotImplementedException($"Projectile not known : {projType}");
        }
        
        private Animation GetProjectileAnimation(ProjectileAnimation projAnimation)
        {
            switch (projAnimation)
            {
                case ProjectileAnimation.Spark:
                    return ProjSO.SparkProjAnimation;
            }

            throw new NotImplementedException($"Projectile Animation not known : {projAnimation}");
        }

        private Collider2D GetProjectileBasicCollider2D(ProjectileBasicCollider projBasicCollider)
        {
            switch (projBasicCollider)
            {
                case ProjectileBasicCollider.Spark:
                    return ProjSO.SparkCollider2D;
            }
            
            throw new NotImplementedException($"Projectile Basic Collider not known : {projBasicCollider}");
        }
        
        private Collider2D GetProjectileSearchCollider2D(ProjectileSearchCollider projSearchCollider)
        {
            switch (projSearchCollider)
            {
                case ProjectileSearchCollider.FindSender:
                    return ProjSO.CircleSearchSenderCollider2D;
                case ProjectileSearchCollider.Cone:
                    return ProjSO.ConeSearchTargetCollider2D;
            }
            
            throw new NotImplementedException($"Projectile Search Collider not known : {projSearchCollider}");
        }
    }
}