using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Netcode;
using _Scripts.GameManager;
using _Scripts.Multiplayer;

public class PlayerTracking : NetworkBehaviour
{
    public List<GameObject> PlayerList;

    public GameObject GetClosestPlayer(GameObject other) //Assuming the list is not empty
    {
        return (from player in PlayerList
                orderby Vector2.Distance(player.transform.position, other.transform.position)
                select player).FirstOrDefault();

        // return (from pl in players 
        //         orderby Math.Abs(pl.transform.position - other.transform.position) descending
        //         select pl).FirstOrDefault();
    }

    public void SetPlayerList()
    {
        PlayerId[] playerIdScript = FindObjectsByType(typeof(PlayerId), FindObjectsSortMode.None) as PlayerId[];
        PlayerList = (from player in playerIdScript
                        let player_id = player.GetPlayerId()
                        //where player_id != (int)OwnerClientId
                        orderby player_id
                        select player.gameObject).ToList();
    }
}
