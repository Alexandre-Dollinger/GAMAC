using UnityEngine;

namespace _Scripts.Player.ColorSwap
{
    [CreateAssetMenu(menuName = "PlayerColorList")]
    public class PlayerColorList : ScriptableObject
    {
        [Header("Colors")] 
        public Material Player2Color;
        public Material Player3Color;
        public Material Player4Color;
        public Material Player5Color;
        public Material Player6Color;
        public Material Player7Color;
        public Material Player8Color;
    }
}
