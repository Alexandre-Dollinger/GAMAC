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

    public void Update()
    {
        CheckAndRemoveNull();
    }

    public void CheckAndRemoveNull()
    {
        int n = PlayerList.Count;
        for (int i = 0; i < n; i++)
        {
            if (PlayerList[i] == null)
            {
                PlayerList.RemoveAt(i);
                n--;
                i--;
            }
        }
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

    public void ReOrderPlayerList()
    {
        List<GameObject> res = (from player in PlayerList
            let player_id = player.GetComponent<PlayerId>().GetPlayerId()
            orderby player_id
            select player).ToList();

        PlayerList = res;
    }

    public void RemoveIdFromList(int idToRemove)
    {
        List<GameObject> res = (from player in PlayerList
            let player_id = player.GetComponent<PlayerId>().GetPlayerId()
            where player_id != idToRemove
            orderby player_id
            select player).ToList();

        PlayerList = res;
    }
}
