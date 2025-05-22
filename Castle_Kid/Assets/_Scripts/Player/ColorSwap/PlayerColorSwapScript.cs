using System;
using _Scripts.GameManager;
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

        public void SetColorManager(int playerId)
        {
            if (playerId is <= 0 or > 7)
                return;

            for (int id = 1; id <= playerId; id++)
            {
                SetColor(id, true);
            }
            
            SetColorServerRpc(playerId);
        }

        private void SetColor(int playerId, bool locally = false)
        {
            GM.playerTracking.PlayerList[playerId].GetComponent<SpriteRenderer>().material =
                GetPlayerMaterial(playerId);

            _swappedColor = locally;
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
