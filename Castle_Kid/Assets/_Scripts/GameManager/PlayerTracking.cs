using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Netcode;

public class PlayerTracking : NetworkBehaviour
{
    public static List<GameObject> players = new List<GameObject>();

    public static GameObject GetClosestPlayer(GameObject other) //Assuming the list is not empty
    {
        return (from player in players
                orderby Vector2.Distance(player.transform.position, other.transform.position) descending 
                select player).FirstOrDefault();

        // return (from pl in players 
        //         orderby Math.Abs(pl.transform.position - other.transform.position) descending
        //         select pl).FirstOrDefault();
    } 
}
