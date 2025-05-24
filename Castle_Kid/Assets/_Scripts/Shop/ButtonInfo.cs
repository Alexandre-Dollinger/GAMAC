using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



public class ButtonInfo : MonoBehaviour
{
    public int ItemID;
    public Text PriceTxt;
    public Text QuantityTxt;
    public GameObject ShopManager;
    
    void Start()
    {
        if (ItemID <= 0)
        {
            Debug.LogError("ItemID not set correctly on: " + gameObject.name);
            return;
        }

        var shop = ShopManager.GetComponent<ShopManagerScript>();
        PriceTxt.text = "Prix: " + shop.shopItems[2, ItemID] + " écus";
        QuantityTxt.text = shop.shopItems[3, ItemID].ToString();
    }
    void Update()
    {
    }
}


