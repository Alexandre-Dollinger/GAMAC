using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Netcode;

public class PlayerTracking : NetworkBehaviour
{
    public static List<GameObject> PlayerList = new List<GameObject>();

    public static GameObject GetClosestPlayer(GameObject other) //Assuming the list is not empty
    {
        return (from player in PlayerList
                orderby Vector2.Distance(player.transform.position, other.transform.position) descending 
                select player).FirstOrDefault();

        // return (from pl in players 
        //         orderby Math.Abs(pl.transform.position - other.transform.position) descending
        //         select pl).FirstOrDefault();
    } 
}
