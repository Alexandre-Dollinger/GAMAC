using _Scripts.GameManager;
using _Scripts.Player.ColorSwap;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Multiplayer
{
    public class PlayerId : NetworkBehaviour
    {
        private NetworkVariable<int> _playerId = new NetworkVariable<int>();
        public int GetPlayerId()
        {
            return _playerId.Value;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                _playerId.Value = (int)OwnerClientId;

            if (IsOwner)
                GM.playerTracking.SetPlayerList();
            else
            {
                GM.playerTracking.PlayerList.Add(gameObject);
                GM.playerTracking.ReOrderPlayerList();

                foreach (GameObject player in GM.playerTracking.PlayerList)
                {
                    if (player.GetComponent<PlayerId>().IsItMyPlayer())
                        player.GetComponent<PlayerColorSwapScript>().UpdateColor();
                }
            }
            
            if (IsOwner)
                GetComponent<PlayerColorSwapScript>().UpdateColor();
        }

        /*public override void OnNetworkDespawn()
        {
            if (IsOwner)
                RemovePlayerFromListServerRpc(_playerId.Value);
        }

        [ServerRpc]
        private void RemovePlayerFromListServerRpc(int playerId)
        {
            RemovePlayerFromListClientRpc(playerId);
        }*/

        [ClientRpc]
        private void RemovePlayerFromListClientRpc(int playerId)
        {
            GM.playerTracking.RemoveIdFromList(playerId);
        }

        public bool IsItMyPlayer()
        {
            return IsOwner;
        }
    }
}
