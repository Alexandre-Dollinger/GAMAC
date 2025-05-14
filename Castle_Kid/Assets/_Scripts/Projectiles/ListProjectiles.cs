using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Projectiles
{
    [CreateAssetMenu(menuName = "ListProjectiles")]
    public class ListProjectiles : ScriptableObject
    {
        [Header("Projectiles Prefabs")]
        [Space(5)]
        
        [Header("Spark")]
        public GameObject SparkPrefabCone;
        public GameObject SparkPrefabCircle;
        public GameObject ShieldBlueCircle;
        public GameObject FireCone;
        public GameObject FireBallCone;
        public GameObject FireBall2Cone;
        public GameObject TornadoCone;
        public GameObject BlackHoleCone;

    }
}
