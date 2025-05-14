using _Scripts.Enemy;
using Unity.Netcode;
using UnityEngine;
using _Scripts.GameManager;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private EnemyType enemyType;
    private Vector2 closestPlayerPos;
    private float closestPlayerCooldown; //The one that is decreased
    private float closestPlayerTime; //Max time
    private float spawnRange;
    private bool needSpawn;
    private bool hasSpawned = false;


    #region Start and Update
    public override void OnNetworkSpawn()
    {
        closestPlayerTime = 3f;
        closestPlayerCooldown = 0f;
        spawnRange = 300f;
    }

    void Update()
    {
        if (!GM.GameStarted)
            return;

        CountTimers();
        UpdateClosestPlayer();

        if (needSpawn)
        {
            SpawnEnemyServerRpc();
            needSpawn = false; 
        }
    }

    #endregion

    private void UpdateClosestPlayer()
    {
        if (!hasSpawned && closestPlayerCooldown <= 0)
        {
            closestPlayerCooldown = closestPlayerTime;

            GameObject closestPlayer = PlayerTracking.GetClosestPlayer(gameObject);

            if (closestPlayer is not null)
            {
                closestPlayerPos = closestPlayer.transform.position;
                needSpawn = InSpawnRange();
            }
            else 
                needSpawn = false;
        }
    }

    #region SpawnEnemy
    private bool InSpawnRange()
    {
        return Vector2.Distance(closestPlayerPos, gameObject.transform.position) <= spawnRange;
    }

    
    [ServerRpc(RequireOwnership = false)] 
    private void SpawnEnemyServerRpc()
    {
        SpawnEnemyClientRpc();
    }

    [ClientRpc]

    private void SpawnEnemyClientRpc()
    {
        GM.EnemyM.CreateEnemyServerRpc(enemyType, transform.position);
        hasSpawned = true;
    }

    #endregion

    #region Timer
    private void CountTimers()
    {
        float deltaTime = Time.deltaTime;

        if (closestPlayerCooldown > 0)
        {
            closestPlayerCooldown -= deltaTime;
        }
    }
    #endregion
}
