using System.Collections;
using System.Collections.Generic;
using _Scripts.Inputs;
using UnityEngine;
using _Scripts.GameManager;
using _Scripts.Multiplayer;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Emote Animator")]
    [SerializeField] private Animator emoteAnimator;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;

    private void Awake() 
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Update() 
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);
            
            if (InputManager.InteractWasPressed) 
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else 
        {
            visualCue.SetActive(false);
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
        }
    }
}