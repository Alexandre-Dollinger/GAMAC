using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Netcode;
using _Scripts.GameManager;

public class PlayerTracking : NetworkBehaviour
{
    public List<GameObject> PlayerList;

    public GameObject GetClosestPlayer(GameObject other) //Assuming the list is not empty
    {
        return (from player in PlayerList
                orderby Vector2.Distance(player.transform.position, other.transform.position) descending 
                select player).FirstOrDefault();

        // return (from pl in players 
        //         orderby Math.Abs(pl.transform.position - other.transform.position) descending
        //         select pl).FirstOrDefault();
    }

    public void SetPlayerList()
    {
        GameObject[] lstPlayer = GameObject.FindGameObjectsWithTag(GM.PlayerTag);
        PlayerList = lstPlayer.ToList();
    }
}
