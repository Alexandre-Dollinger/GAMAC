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
        if (!IsServer)
            gameObject.SetActive(false);

        closestPlayerTime = 3f;
        closestPlayerCooldown = 0f;
        spawnRange = 300f;
    }

    void FixedUpdate()
    {
        if (!GM.GameStarted)
            return;

        UpdateClosestPlayer();

        if (needSpawn)
        {
            SpawnEnemy();   
            needSpawn = false; 
        }
    }

    void Update()
    {
        if (!GM.GameStarted)
            return;

        CountTimers();
    }

    #endregion

    private void UpdateClosestPlayer()
    {
        if (!hasSpawned && closestPlayerCooldown <= 0)
        {
            closestPlayerCooldown = closestPlayerTime;

            GameObject closestPlayer = GM.playerTracking.GetClosestPlayer(gameObject);

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

    private void SpawnEnemy()
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
