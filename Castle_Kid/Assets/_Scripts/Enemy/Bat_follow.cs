using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Bat_follow : MonoBehaviour
{
    public float speed;
    private GameObject player;
    
    
    //Searching for the player
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        Chase();
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
