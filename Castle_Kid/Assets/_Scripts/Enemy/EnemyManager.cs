using System;
using System.Collections.Generic;
using _Scripts.GameManager;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Enemy
{
    public enum EnemyType
    {
        Slime,
    }

    public class EnemyManager : NetworkBehaviour 
    {
        #region EnemyPrefab
        public GameObject SlimePrefab;
        #endregion

        public GameObject GetEnemyPrefab(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Slime:
                    return SlimePrefab;
            }

            throw new NotImplementedException($"Enemy not found : {enemyType}");
        }

        [ServerRpc(RequireOwnership = false)] 

        public void CreateEnemyServerRpc(EnemyType enemyType)
        {
            GameObject gameObjectEnemy = Instantiate(GetEnemyPrefab(enemyType));

            gameObjectEnemy.GetComponent<NetworkObject>().Spawn(true);
            gameObjectEnemy.tag = GM.EnemyTag;
        }


    }
}