using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.Player.PowerUp
{
    public class PowerUp : MonoBehaviour
    {
        [Header("References")]
        public PowerUpStats powerStats;

        public void Update()
        {
            if (InputManager.PowerUp1WasReleased)
            {
                Vector3 offsetSpawn = new Vector3(50,0,0);
                GM.ProjM.CreateProjectile(ProjectilesType.Spark, GM.PlayerAttackTag, 50, 50, 
                    transform.position + offsetSpawn, Vector3.right, 
                    GM.IsTargetForEnemy);
            }
        }
    }
}
