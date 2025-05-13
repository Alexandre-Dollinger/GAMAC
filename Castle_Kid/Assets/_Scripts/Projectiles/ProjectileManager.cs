using System;
using System.Collections.Generic;
using _Scripts.GameManager;
using JetBrains.Annotations;
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
        private bool _attackedLocally = false;

        #region Damage a projectile not Network

        public int GetProjLstId(Projectile projectile)
        {
            int id = 0;
            int n = projList.Count;
            while (id < n && projList[id] != projectile)
            {
                id++;
            }

            if (id < n)
                return id;

            throw new ArgumentException("The projectile couldn't be found in the list for player " + OwnerClientId);
        }
        public void DoDamageToProjManager(int lstId, int attack)
        {
            DoDamageToProjList(lstId, attack, true);
            DoDamageToProjListServerRpc(lstId, attack);
        }
        
        private void DoDamageToProjList(int lstId, int attack, bool locally = false)
        {
            _attackedLocally = locally;
            projList[lstId].TakeDamage(attack);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DoDamageToProjListServerRpc(int lstId, int attack)
        {
            DoDamageToProjListClientRpc(lstId, attack);
        }

        [ClientRpc]
        private void DoDamageToProjListClientRpc(int lstId, int attack)
        {
            if (_attackedLocally)
            {
                _attackedLocally = false;
                return;
            }
            
            DoDamageToProjList(lstId, attack);
        }
        #endregion
        
        
        #region Create projectile not Network
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
            
            projectile.InitProjectile(projStruct, IsServer);
            
            projList.Add(projectile);
        }

        public void CreateProjectileManager(ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset = 50)
        { // for now we don't care about server prediction but we will probably need it : int startTickServer, int startTickClient, | NetworkManager.Singleton.ServerTime.Tick, NetworkManager.Singleton.LocalTime.Tick,
            if (projTag != GM.PlayerProjectileTag)
                playerId = -1;

            CreateProjectileLocally(projStruct, projPrefabType, projTag, playerId, offset);
            CreateProjectileServerRpc(projStruct, projPrefabType, projTag, playerId, offset);
        }

        private void CreateProjectileLocally(ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset)
        { 
            CreateProjectile(projStruct, projPrefabType, projTag, playerId, offset);
            
            _spawnedLocally = true;
        }

        [ClientRpc]
        private void CreateProjectileClientRpc(ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset)
        {
            if (_spawnedLocally) // to exclude the client that spawned locally
            {
                _spawnedLocally = false;
                return;
            }
            
            CreateProjectile(projStruct, projPrefabType, projTag, playerId, offset);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreateProjectileServerRpc(ProjectileStruct projStruct,
            ProjectilePrefabType projPrefabType, string projTag, int playerId, float offset)
        {
            CreateProjectileClientRpc(projStruct, projPrefabType, projTag, playerId, offset);
        }
        #endregion

        #region References getters for projectile
        public GameObject GetProjectilePrefab(ProjectilePrefabType projType)
        {
            switch (projType)
            {
                case ProjectilePrefabType.Spark:
                    return sparkPrefab;
            }

            throw new NotImplementedException($"Projectile not known : {projType}");
        }
        
        public RuntimeAnimatorController GetProjectileAnimation(ProjectileAnimation projAnimation)
        {
            switch (projAnimation)
            {
                case ProjectileAnimation.Spark:
                    return ProjSO.SparkProjAnimation;
            }

            throw new NotImplementedException($"Projectile Animation not known : {projAnimation}");
        }

        public Collider2D GetProjectileBasicCollider2D(ProjectileBasicCollider projBasicCollider)
        {
            switch (projBasicCollider)
            {
                case ProjectileBasicCollider.Spark:
                    return ProjSO.SparkCollider2D;
            }
            
            throw new NotImplementedException($"Projectile Basic Collider not known : {projBasicCollider}");
        }
        
        [CanBeNull]
        public Collider2D GetProjectileSearchCollider2D(ProjectileSearchCollider projSearchCollider)
        {
            switch (projSearchCollider)
            {
                case ProjectileSearchCollider.None:
                    return null;
                case ProjectileSearchCollider.FindSender:
                    return ProjSO.CircleSearchSenderCollider2D; 
                case ProjectileSearchCollider.Cone:
                    return ProjSO.ConeSearchTargetCollider2D;
            }
            
            throw new NotImplementedException($"Projectile Search Collider not known : {projSearchCollider}");
        }
        #endregion
        
        public void DebugTickServerAndClient(int startTickServer, int startTickClient)
        {
            float timeDifferenceServer = (float)(NetworkManager.Singleton.ServerTime.Tick - startTickServer) / 
                                         NetworkManager.Singleton.ServerTime.TickRate;
            float timeDifferenceClient = (float)(NetworkManager.Singleton.LocalTime.Tick - startTickClient) /
                                         NetworkManager.Singleton.LocalTime.TickRate;
            Debug.Log($"Server : Called num {OwnerClientId} with time difference : {timeDifferenceServer} || Current Tick : {NetworkManager.Singleton.ServerTime.Tick}, start Tick : {startTickServer}");
            Debug.Log($"Client : Called num {OwnerClientId} with time difference : {timeDifferenceClient} || Current Tick : {NetworkManager.Singleton.LocalTime.Tick}, start Tick : {startTickClient}");
        }
    }
}