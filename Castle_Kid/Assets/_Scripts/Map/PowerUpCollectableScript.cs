using System;
using System.Collections.Generic;
using _Scripts.GameManager;
using _Scripts.Multiplayer;
using _Scripts.Player.PowerUp;
using _Scripts.Projectiles;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Map
{
    public class PowerUpCollectableScript : NetworkBehaviour
    {
        [SerializeField] private ProjectileStructEnum projectileStructEnum;
        [SerializeField] private ProjectilePrefabs projectilePrefabs;
        [SerializeField] private float cooldown;

        [SerializeField] private SpriteRenderer powerUpImage;

        public override void OnNetworkSpawn()
        {
            GameObject projPrefab = GM.ProjM.GetProjectilePrefab(projectilePrefabs);
            powerUpImage.sprite = projPrefab.GetComponent<SpriteRenderer>().sprite;
            powerUpImage.transform.localScale *= projPrefab.transform.localScale.x;
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsServer)
                return;
            
            if (GM.IsPlayer(other) && other.transform.parent.parent.TryGetComponent(out PlayerId playerId))
            {
                CollectPowerUpClientRpc(playerId.GetPlayerId(), clientRpcParams: new ClientRpcParams() 
                    {Send = new ClientRpcSendParams(){TargetClientIds = new List<ulong>{ (ulong)playerId.GetPlayerId() }}});
            }
        }
        
        [ClientRpc]
        private void CollectPowerUpClientRpc(int sendFor, ClientRpcParams clientRpcParams = default)
        {
            PowerUpStruct powerUpStruct = new PowerUpStruct(GM.ProjM.GetProjectileStruct(projectileStructEnum), projectilePrefabs, cooldown);
            GM.playerTracking.GetPlayerWithId(sendFor).GetComponent<PowerUp>().UpdatePowerUp(powerUpStruct);
            DestroyPowerUpServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DestroyPowerUpServerRpc()
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}
