using _Scripts.Health;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace _Scripts.Zones
{
    public class DeathZonesScript : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent<UnitHp>(out UnitHp otherHp))
            {
                otherHp.Die();
            }
            else if (other.gameObject.TryGetComponent<UnitLocalHp>(out UnitLocalHp otherLocalHp))
            {
                otherLocalHp.Die();
            }
        }
    }
}
