using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Multiplayer
{
    public class PlayerId : NetworkBehaviour
    {
        private NetworkVariable<ulong> _playerId = new NetworkVariable<ulong>();
        public ulong GetPlayerId()
        {
            return _playerId.Value;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                SetPlayerIdServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIdServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (IsServer)
                _playerId.Value = serverRpcParams.Receive.SenderClientId;
        }
    }
}
