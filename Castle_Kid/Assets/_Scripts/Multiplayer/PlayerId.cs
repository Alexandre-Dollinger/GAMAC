using Unity.Netcode;

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
            if (IsOwner)
                SetPlayerIdServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIdServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (IsServer)
                _playerId.Value = (int)serverRpcParams.Receive.SenderClientId;
        }
        
        public bool IsItMyPlayer()
        {
            return IsOwner;
        }
    }
}
