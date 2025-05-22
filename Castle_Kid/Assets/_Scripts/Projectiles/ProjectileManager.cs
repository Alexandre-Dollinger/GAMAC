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
        public ListProjectiles lstProjPrefabs;

        public List<Projectile> projListSpawned; // A list only for the owner

        private bool _spawnedLocally = false; // to exclude the one that spawned locally from the clientRpc
        private bool _changedHpLocally = false;

        #region Damage a projectile not Network
        
        public int GetProjLstId(Projectile projectile)
        {
            int id = 0;
            int n = projListSpawned.Count;
            while (id < n && projListSpawned[id] != projectile)
            {
                id++;
            }

            if (id < n)
                return id;

            throw new ArgumentException("The projectile couldn't be found in the list for player " + OwnerClientId);
        }
        public void ChangeProjHpManager(int lstId, int value, bool isHealing = false)
        {
            ChangeProjHpList(lstId, value, isHealing, true);
            DoDamageToProjListServerRpc(lstId, value, isHealing);
        }
        
        private void ChangeProjHpList(int lstId, int value, bool isHealing, bool locally = false)
        {
            _changedHpLocally = locally;
            
            if (!isHealing)
                projListSpawned[lstId].TakeDamage(value);
            else
                projListSpawned[lstId].GainHealth(value);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DoDamageToProjListServerRpc(int lstId, int value, bool isHealing)
        {
            ChangeProjHpListClientRpc(lstId, value, isHealing);
        }

        [ClientRpc]
        private void ChangeProjHpListClientRpc(int lstId, int value, bool isHealing)
        {
            if (_changedHpLocally)
            {
                _changedHpLocally = false;
                return;
            }
            
            ChangeProjHpList(lstId, value, isHealing);
        }
        #endregion

        #region Destroy On (following) Projectiles

        [ServerRpc(RequireOwnership = false)]
        public void DestroyFollowProjectileServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer)
                return;
            
            List<int> projToDestroy = GetFollowProjectileLstId(serverRpcParams.Receive.SenderClientId);

            foreach (int idToDestroy in projToDestroy)
            {
                DestroyGivenIdProjClientRpc(idToDestroy);
            }
        }

        private List<int> GetFollowProjectileLstId(ulong playerConcerned)
        {
            List<int> res = new List<int>();

            int listCount = projListSpawned.Count;
            for (int id = listCount - 1; id >= 0; id--)
            {
                if (projListSpawned[id].Proj.AttackType is ProjectileAttackTypes.AroundSender or ProjectileAttackTypes.OnSender)
                {
                    if (projListSpawned[id].Proj.SenderId == (int)playerConcerned)
                        res.Add(id);
                        
                }
            }

            return res;
        }

        [ClientRpc]
        private void DestroyGivenIdProjClientRpc(int projId)
        {
            projListSpawned[projId].Die(); // automatically removes it from the list
        }
        #endregion
        
        #region Create projectile not Network

        private void CreateProjectile(ProjectileStruct projStruct,
            ProjectilePrefabs projPrefab, string projTag, int playerId)
        {
            GameObject gameObjectProj = Instantiate(GetProjectilePrefab(projPrefab), 
                projStruct.SpawnPos, Quaternion.identity);
            
            //gameObjectProj.GetComponent<NetworkObject>().Spawn(true);
            
            gameObjectProj.tag = projTag;

            Projectile projectile = gameObjectProj.GetComponent<Projectile>();

            if (projTag == GM.PlayerProjectileTag && playerId != -1) // if it does not come from a player then we don't care about it's sender Id 
                projStruct.SenderId = playerId;
            
            projectile.InitProjectile(projStruct, IsServer);
            
            projListSpawned.Add(projectile);
        }

        public void CreateProjectileManager(ProjectileStruct projStruct,
            ProjectilePrefabs projPrefab, string projTag, int playerId)
        { // for now we don't care about server prediction but we will probably need it : int startTickServer, int startTickClient, | NetworkManager.Singleton.ServerTime.Tick, NetworkManager.Singleton.LocalTime.Tick,
            if (projTag != GM.PlayerProjectileTag)
                playerId = -1;

            CreateProjectileLocally(projStruct, projPrefab, projTag, playerId);
            CreateProjectileServerRpc(projStruct, projPrefab, projTag, playerId);
        }

        private void CreateProjectileLocally(ProjectileStruct projStruct,
            ProjectilePrefabs projPrefab, string projTag, int playerId)
        { 
            CreateProjectile(projStruct, projPrefab, projTag, playerId);
            
            _spawnedLocally = true;
        }

        [ClientRpc]
        private void CreateProjectileClientRpc(ProjectileStruct projStruct,
            ProjectilePrefabs projPrefab, string projTag, int playerId)
        {
            if (_spawnedLocally) // to exclude the client that spawned locally
            {
                _spawnedLocally = false;
                return;
            }
            
            CreateProjectile(projStruct, projPrefab, projTag, playerId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreateProjectileServerRpc(ProjectileStruct projStruct,
            ProjectilePrefabs projPrefab, string projTag, int playerId)
        {
            CreateProjectileClientRpc(projStruct, projPrefab, projTag, playerId);
        }
        #endregion

        #region Getter for projectile Prefabs
        public GameObject GetProjectilePrefab(ProjectilePrefabs projType)
        {
            switch (projType)
            {
                case ProjectilePrefabs.SparkCone:
                    return lstProjPrefabs.SparkPrefabCone;
                case ProjectilePrefabs.SparkCircle:
                    return lstProjPrefabs.SparkPrefabCircle;
                case ProjectilePrefabs.ShieldBlueCircle:
                    return lstProjPrefabs.ShieldBlueCircle;
                case ProjectilePrefabs.FireCone:
                    return lstProjPrefabs.FireCone;
                case ProjectilePrefabs.FireBallCone:
                    return lstProjPrefabs.FireBallCone;
                case ProjectilePrefabs.FireBall2Cone:
                    return lstProjPrefabs.FireBall2Cone;
                case ProjectilePrefabs.TornadoCone:
                    return lstProjPrefabs.TornadoCone;
                case ProjectilePrefabs.BlackHoleCone:
                    return lstProjPrefabs.BlackHoleCone;
                case ProjectilePrefabs.BubblesCone:
                    return lstProjPrefabs.BubblesCone;
                case ProjectilePrefabs.MagicArrowCone:
                    return lstProjPrefabs.MagicArrowCone;
                case ProjectilePrefabs.MagicBallCone:
                    return lstProjPrefabs.MagicBallCone;
                case ProjectilePrefabs.MagicBulletCone:
                    return lstProjPrefabs.MagicBulletCone;
                case ProjectilePrefabs.RocksCone:
                    return lstProjPrefabs.RocksCone;
                case ProjectilePrefabs.ShieldPlusCircle:
                    return lstProjPrefabs.ShieldPlusCircle;
                case ProjectilePrefabs.WindCone:
                    return lstProjPrefabs.WindCone;
            }

            throw new NotImplementedException($"Projectile not known : {projType}");
        }
        #endregion

        public string GetSenderTag(SenderTags senderTag)
        {
            switch (senderTag)
            {
                case SenderTags.Player:
                    return GM.PlayerTag;
                case SenderTags.Enemy:
                    return GM.EnemyTag;
                case SenderTags.PlayerProjectile:
                    return GM.PlayerProjectileTag;
                case SenderTags.EnemyProjectile:
                    return GM.EnemyProjectileTag;
            }

            throw new ArgumentException("What is that SenderTag : " + senderTag);
        }
        
        public void DebugTickServerAndClient(int startTickServer, int startTickClient)
        {
            float timeDifferenceServer = (float)(NetworkManager.Singleton.ServerTime.Tick - startTickServer) / 
                                         NetworkManager.Singleton.ServerTime.TickRate;
            float timeDifferenceClient = (float)(NetworkManager.Singleton.LocalTime.Tick - startTickClient) /
                                         NetworkManager.Singleton.LocalTime.TickRate;
            Debug.Log($"Server : Called num {OwnerClientId} with time difference : {timeDifferenceServer} || Current Tick : {NetworkManager.Singleton.ServerTime.Tick}, start Tick : {startTickServer}");
            Debug.Log($"Client : Called num {OwnerClientId} with time difference : {timeDifferenceClient} || Current Tick : {NetworkManager.Singleton.LocalTime.Tick}, start Tick : {startTickClient}");
        }
        
        public ProjectileStruct GetProjectileStruct(ProjectileStructEnum projectileStructEnum)
        {
            switch (projectileStructEnum)
            {
                case ProjectileStructEnum.LinearDamage:
                    return GM.GetLinearProjectileStruct();
                case ProjectileStructEnum.LinearHealing:
                    ProjectileStruct lH = GM.GetLinearProjectileStruct();
                    lH.InitHealing();
                    return lH;
                case ProjectileStructEnum.LinearAcceleratingDamage:
                    ProjectileStruct lAD = GM.GetLinearProjectileStruct();
                    lAD.InitSpeed(50, 50, 125);
                    lAD.CanCrossWalls = true;
                    return lAD;
                case ProjectileStructEnum.LinearAcceleratingHealing:
                    ProjectileStruct lAH = GetProjectileStruct(ProjectileStructEnum.LinearAcceleratingDamage);
                    lAH.InitHealing();
                    return lAH;
                case ProjectileStructEnum.TrackingDamage:
                    ProjectileStruct tD = GM.GetTrackingProjectileStruct();
                    return tD;
                case ProjectileStructEnum.TrackingHealing:
                    ProjectileStruct tH = GM.GetTrackingProjectileStruct();
                    tH.InitHealing();
                    return tH;
                case ProjectileStructEnum.TrackingAcceleratingDamage:
                    ProjectileStruct tAD = GM.GetTrackingProjectileStruct();
                    tAD.InitSpeed(50, 50, 125);
                    return tAD;
                case ProjectileStructEnum.TrackingAcceleratingHealing:
                    ProjectileStruct tAH = GM.GetTrackingProjectileStruct();
                    tAH.InitSpeed(50, 50 ,250);
                    tAH.InitHealing();
                    return tAH;
                case ProjectileStructEnum.OnSenderRotating:
                    ProjectileStruct sR = GM.GetOnSenderProjectileStruct();
                    sR.RotateSpeed = 180f;
                    return sR;
                case ProjectileStructEnum.OnSender:
                    ProjectileStruct s = GM.GetOnSenderProjectileStruct();
                    return s;
                case ProjectileStructEnum.AroundSenderRotating:
                    ProjectileStruct aSR = GM.GetAroundSenderProjectileStruct();
                    return aSR;
                case ProjectileStructEnum.AroundSenderRotatingSelf:
                    ProjectileStruct aSRS = GM.GetAroundSenderProjectileStruct();
                    aSRS.RotateSpeed = 180f;
                    return aSRS;
                case ProjectileStructEnum.AroundSender:
                    ProjectileStruct aS = GM.GetAroundSenderProjectileStruct();
                    aS.RotateSpeed = 0f;
                    aS.RotateAroundSpeed = 0f;
                    return aS;
                case ProjectileStructEnum.Fix:
                    ProjectileStruct f = GM.GetFixProjectileStruct();
                    return f;
                case ProjectileStructEnum.FixHealing:
                    ProjectileStruct fH = GM.GetFixProjectileStruct();
                    fH.InitHealing();
                    return fH;
            }

            throw new ArgumentException("Don't know this Projectile Struct : " + projectileStructEnum);
        }
    }
}