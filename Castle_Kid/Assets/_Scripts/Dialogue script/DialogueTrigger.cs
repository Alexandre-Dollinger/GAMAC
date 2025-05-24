using System.Collections;
using System.Collections.Generic;
using _Scripts.Inputs;
using UnityEngine;
using _Scripts.GameManager;
using _Scripts.Multiplayer;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject ShopPanel;
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Emote Animator")]
    [SerializeField] private Animator emoteAnimator;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;
    
    private bool shopIsOpen = false;
    
    private float shopOpenTime = 0f;


    

    private void Awake() 
    {
        playerInRange = false;
        visualCue.SetActive(false);
        ShopPanel.SetActive(false);
        shopIsOpen = false;

    }

    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);

            if (InputManager.InteractWasPressed)
            {
                if (CompareTag("ShopNPC"))
                {
                    if (ShopPanel != null)
                    {
                        if (!shopIsOpen)
                        {
                            shopIsOpen = true;
                            shopOpenTime = 0f;
                            ShopPanel.SetActive(true);
                        }
                        else if (shopOpenTime >= 1f)
                        {
                            shopIsOpen = false;
                            ShopPanel.SetActive(false);
                        }
                    }
                }
                else if (inkJSON != null)
                {
                    DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
                }
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
        if (shopIsOpen)
        {
            shopOpenTime += Time.deltaTime;
        }
        
    }

    

    private void OnTriggerEnter2D(Collider2D collider) 
        {
            if (collider.gameObject.tag == "Player")
            {
                playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collider) 
        {
            if (collider.CompareTag("Player") && collider.transform.parent.parent.GetComponent<PlayerId>().IsItMyPlayer())
            {
                playerInRange = false;
                GM.dialogueManager.ExitDialogueMode();
                if (gameObject.name == "ShopNPC" && ShopPanel != null)
                {
                    ShopPanel.SetActive(false);
                    shopIsOpen = false;
                }
            }
            if (collider.CompareTag("Player") && CompareTag("ShopNPC") && ShopPanel != null)
            {
                ShopPanel.SetActive(false);
                shopIsOpen = false;
            }
        }
}