using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerStats : MonoBehaviour
    {
        public int MaxHp;
        public int Hp;

        public void GetHit(int damage)
        {
            Debug.Log("Player got hit !");
            Hp -= damage;
            if (Hp <= 0)
                GetKilled();
        }
        public void GetKilled()
        {
            Hp = MaxHp;
            transform.position = new Vector3(0,0,0);
        }
    }
}
