using System;
using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Projectiles;
using _Scripts.UI_scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class DestroyObject : MonoBehaviour
{
    [SerializeField] private int minCoins = 1;
    [SerializeField] private int maxCoins = 8;
    [SerializeField] private GameObject woodParticlesPrefab;


    private bool isDestroyed = false;

    public void Break()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        int coinsToGive = Random.Range(minCoins, maxCoins + 1);

        ShopManagerScript shop = FindObjectOfType<ShopManagerScript>();
        if (shop != null)
        {
            shop.AddCoins(coinsToGive);
        }
        else
        {
        }
        
        Destroy(gameObject);
        if (woodParticlesPrefab != null)
        {
            var particles = Instantiate(woodParticlesPrefab, transform.position, Quaternion.identity);
                 Destroy(particles, 2f);
        }
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Break();
        }
    }
}
