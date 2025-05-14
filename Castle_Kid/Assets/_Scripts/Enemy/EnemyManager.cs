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

        public void CreateEnemyServerRpc(EnemyType enemyType, Vector2 position)
        {
            GameObject gameObjectEnemy = Instantiate(GetEnemyPrefab(enemyType), position, Quaternion.identity);

            gameObjectEnemy.GetComponent<NetworkObject>().Spawn(true);
            Debug.Log("Enemy Spawned");
            gameObjectEnemy.tag = GM.EnemyTag;
        }


    }
}