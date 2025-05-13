using UnityEngine;

namespace _Scripts.Projectiles
{
    [CreateAssetMenu(menuName = "ProjectileScriptableObject")]
    public class ProjectileScriptableObject : ScriptableObject
    {
        [Header("Projectile Sprite and Animation")]
        public Animation SparkProjAnimation;
        [Space(5)]
        
        [Header("Projectile Basic Collider")] 
        public Collider2D SparkCollider2D;
        [Space(5)] 
        
        [Header("Projectile Search Collider")]
        public Collider2D ConeSearchTargetCollider2D;
        public Collider2D CircleSearchSenderCollider2D;
        //[Space(5)]
    }
}
