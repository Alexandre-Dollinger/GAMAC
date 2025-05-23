using System;
using _Scripts.GameManager;
using _Scripts.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Player.ColorSwap
{
    public class PlayerColorSwapScript : NetworkBehaviour
    {
        [SerializeField] private PlayerColorList playerColors;
        private bool _swappedColor = false;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;
        }
        
        /*
        public void SetColorManager()
        {
            int playerListCount = GM.playerTracking.PlayerList.Count;
            Debug.Log("Player list count for newcomer : " + playerListCount);
            
            if (playerListCount is <= 1 or > 8)
                return;

            for (int id = 1; id < playerListCount; id++)
            {
                SetColor(id, true);
            }
            
            SetColorServerRpc(playerListCount - 1);
        }
        
        [ServerRpc]
        private void SetColorServerRpc(int playerId)
        {
            SetColorClientRpc(playerId);
        }

        [ClientRpc]
        private void SetColorClientRpc(int playerId)
        {
            if (_swappedColor)
            {
                _swappedColor = false;
                return;
            }
            
            SetColor(playerId);
        }
        */
        
        private void SetColor(int playerId, bool locally = false)
        {
            GM.playerTracking.PlayerList[playerId].GetComponent<SpriteRenderer>().material =
                GetPlayerMaterial(playerId);

            _swappedColor = locally;
        }
        
        public void UpdateColor()
        {
            int playerListCount = GM.playerTracking.PlayerList.Count;
            
            for (int id = 1; id < playerListCount; id++)
            {
                SetColor(id);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void UpdateColorServerRpc()
        {
            UpdateColorClientRpc();
        }

        [ClientRpc]
        private void UpdateColorClientRpc()
        {
            UpdateColor();
        }

        private Material GetPlayerMaterial(int playerId)
        {
            switch (playerId + 1)
            {
                case 2:
                    return playerColors.Player2Color;
                case 3:
                    return playerColors.Player3Color;
                case 4:
                    return playerColors.Player4Color;
                case 5:
                    return playerColors.Player5Color;
                case 6:
                    return playerColors.Player6Color;
                case 7:
                    return playerColors.Player7Color;
                case 8:
                    return playerColors.Player8Color;
            }

            throw new ArgumentException("There cannot be that many player, should have been stopped : " + playerId);
        }
    }
}
