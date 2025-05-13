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

        private bool _spawnedLocally = false; // to exclude the one that spawned locally from the clientRpc
 
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
        
        private void CreateProjectile(ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset)
        {
            projStruct.SpawnPos += projStruct.Direction * offset;
            // To not spawn the projectile in our caster

            GameObject gameObjectProj = Instantiate(GetProjectilePrefab(projPrefabType), 
                projStruct.SpawnPos, Quaternion.identity);
            
            //gameObjectProj.GetComponent<NetworkObject>().Spawn(true);
            
            gameObjectProj.tag = projTag;

            Projectile projectile = gameObjectProj.GetComponent<Projectile>();

            if (projTag == GM.PlayerProjectileTag && playerId != -1) // if it does not come from a player then we don't care about it's sender Id 
                projStruct.SenderId = playerId;
            
            projectile.InitProjectile(projStruct);
            
            projList.Add(projectile);
        }

        public void CreateProjectileManager(int startTickServer, int startTickClient, ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset = 50)
        {
            if (projTag != GM.PlayerProjectileTag)
                playerId = -1;

            CreateProjectileLocally(startTickServer, startTickClient, projStruct, projPrefabType, projTag, playerId, offset);
            CreateProjectile2ServerRpc(startTickServer, startTickClient, projStruct, projPrefabType, projTag, playerId, offset);
        }

        private void CreateProjectileLocally(int startTickServer, int startTickClient, ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset)
        { 
            CreateProjectile(projStruct, projPrefabType, projTag, playerId, offset);
            
            Debug.Log(playerId);
            
            Debug.Log("Spawned Locally");
            float timeDifferenceServer = (float)(NetworkManager.Singleton.ServerTime.Tick - startTickServer) / 
                                         NetworkManager.Singleton.ServerTime.TickRate;
            float timeDifferenceClient = (float)(NetworkManager.Singleton.LocalTime.Tick - startTickClient) /
                                         NetworkManager.Singleton.LocalTime.TickRate;
            Debug.Log($"Server : Called num {OwnerClientId} with time difference : {timeDifferenceServer} || Current Tick : {NetworkManager.Singleton.ServerTime.Tick}, start Tick : {startTickServer}");
            Debug.Log($"Client : Called num {OwnerClientId} with time difference : {timeDifferenceClient} || Current Tick : {NetworkManager.Singleton.LocalTime.Tick}, start Tick : {startTickClient}");

            _spawnedLocally = true;
        }

        [ClientRpc]
        private void CreateProjectileClientRpc(int startTickServer, int startTickClient, ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset)
        {
            if (_spawnedLocally) // to exclude the client that spawned locally
            {
                _spawnedLocally = false;
                return;
            }
            
            Debug.Log(playerId);

            CreateProjectile(projStruct, projPrefabType, projTag, playerId, offset);
            
            float timeDifferenceServer = (float)(NetworkManager.Singleton.ServerTime.Tick - startTickServer) / 
                                         NetworkManager.Singleton.ServerTime.TickRate;
            float timeDifferenceClient = (float)(NetworkManager.Singleton.LocalTime.Tick - startTickClient) /
                                         NetworkManager.Singleton.LocalTime.TickRate;
            Debug.Log($"Server : Called num {OwnerClientId} with time difference : {timeDifferenceServer} || Current Tick : {NetworkManager.Singleton.ServerTime.Tick}, start Tick : {startTickServer}");
            Debug.Log($"Client : Called num {OwnerClientId} with time difference : {timeDifferenceClient} || Current Tick : {NetworkManager.Singleton.LocalTime.Tick}, start Tick : {startTickClient}");
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreateProjectile2ServerRpc(int startTickServer, int startTickClient, ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset)
        {
            CreateProjectileClientRpc(startTickServer, startTickClient, projStruct, projPrefabType, projTag, playerId, offset);
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