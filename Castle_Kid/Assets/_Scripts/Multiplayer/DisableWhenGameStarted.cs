using System;
using UnityEngine;
using _Scripts.GameManager;

namespace _Scripts.Multiplayer
{
    public class DisableWhenGameStarted : MonoBehaviour
    {
        public void Update()
        {
            if (GM.GameStarted)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
