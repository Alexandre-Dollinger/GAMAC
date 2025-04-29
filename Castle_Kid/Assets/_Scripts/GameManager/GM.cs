using UnityEngine;

public class GM : MonoBehaviour
{
    public static readonly string PlayerTag = "Player";
    public static readonly string PlayerAttackTag = "PlayerAttack";
    public static readonly string PlayerProjectileTag = "PlayerProjectile";
    public static readonly string EnemyTag = "Enemy";
    public static readonly string EnemyAttackTag = "EnemyAttack";
    public static readonly string EnemyProjectileTag = "EnemyProjectile";
    public static readonly int GroundLayerId = 3;
    
    public static bool IsTargetForPlayer(Collider2D item)
    {
        return item.CompareTag(EnemyTag) ||
               item.CompareTag(EnemyProjectileTag);
    }

    public static bool IsTargetForEnemy(Collider2D item)
    {
        return item.CompareTag(PlayerTag) ||
               item.CompareTag(PlayerProjectileTag);
    }
}
