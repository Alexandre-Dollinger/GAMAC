using UnityEngine;
using System.Collections.Generic;

public class PlayerTracking : MonoBehaviour
{
    public static List<GameObject> players = new List<GameObject>();

    public static GameObject GetClosestPlayer(GameObject other) //Assuming the list is not empty
    {
        GameObject closest = players[0];
        float distance = Vector2.Distance(closest.transform.position, other.transform.position);

        for (int i = 1; i < players.Count; i++)
        {
            float otherDistance = Vector2.Distance(players[i].transform.position, other.transform.position);
            if (distance < otherDistance)
            {
                (closest, distance) = (players[i], otherDistance);
            }
        }
        
        return closest;
    }


}
