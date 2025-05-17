using _Scripts.GameManager;
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
            if (IsServer)
                _playerId.Value = (int)OwnerClientId;
            
            if (IsOwner)
                GM.playerTracking.SetPlayerList();  
            else
                GM.playerTracking.PlayerList.Add(gameObject);
        }
        
        public bool IsItMyPlayer()
        {
            return IsOwner;
        }
    }
}
