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
        public GameObject SparkPrefabConeTrack;
        public GameObject SparkPrefabCircleTrack;
        [Space(5)] 
        
        [Header("Shield")] 
        public GameObject ShieldPrefab;

    }
}
