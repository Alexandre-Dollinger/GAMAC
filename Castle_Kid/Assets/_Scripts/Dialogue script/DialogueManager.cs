using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Inputs;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")] [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")] [SerializeField]
    private GameObject[] choices;
    
    [Header("Dialogue Flow")]
    [SerializeField] private bool canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;

    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }

    private static DialogueManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }

        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        canContinueToNextLine = false;
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;

        }
        
        
        for (int i = 0; i < choices.Length; i++)
        {
            int index2 = i; 
            Button button = choices[i].GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => {
                    if (canContinueToNextLine) {
                        MakeChoice(index2);
                        Debug.Log($"Choice {index2} selected");
                    }
                });
            }
            else
            {
                Debug.LogError($"Choice {i} is missing Button component");
            }
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        
        if (canContinueToNextLine && InputManager.InteractWasPressed)
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        Debug.Log("Dialogue panel enabled: " + dialoguePanel.activeSelf); 
        
        
        ContinueStory();
    }

    public void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
        
            string nextLine = currentStory.Continue();
            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        }
        else
        {
            ExitDialogueMode();
        }
    }
    
    private IEnumerator DisplayLine(string line)
    {
        canContinueToNextLine = false;
    
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(0.02f); 
        }
    
        canContinueToNextLine = true; 
        DisplayChoices();
    }

    private void DisplayChoices() 
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        
        foreach (GameObject choice in choices)
        {
            choice.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        int index = 0;
        foreach(Choice choice in currentChoices) 
        {
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            
            int currentIndex = index; 
            choices[index].GetComponent<Button>().onClick.AddListener(() => {
                if (canContinueToNextLine) {
                    MakeChoice(currentIndex);
                }
            });
        
            index++;
        }
        
        for (int i = index; i < choices.Length; i++) 
        {
            choices[i].SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
        
        for (int i = 0; i < currentChoices.Count; i++)
        {
            choices[i].GetComponent<Button>().interactable = canContinueToNextLine;
        }
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        Debug.Log($"Attempting choice {choiceIndex} - canContinue: {canContinueToNextLine}");
    
        if (canContinueToNextLine)
        {
            Debug.Log($"Making choice {choiceIndex}");
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
        else
        {
            Debug.LogWarning("Choice blocked - text still animating or dialogue not active");
        }
    }
}


