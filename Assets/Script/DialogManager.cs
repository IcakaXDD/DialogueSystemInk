using TMPro;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Collections;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DialogManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Gloabals Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;
    [Header("Choises UI")]
    [SerializeField] private GameObject[] choises;

    private TextMeshProUGUI[] choisesText;
    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }

    private Coroutine displayLineCoroutine;

    private bool canContinueToNextLine = false;

    private static DialogManager Instance;

    private const string SPEAKER_TAG = "speaker";

    private const string PORTRAIT_TAG = "portrait";

    private const string LAYOUT_TAG = "layout";

    private DialogueVariables dialogueVariables;

    private InkExternalFunctions externalFunctions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple DialogManagers!");
            Destroy(gameObject); // Destroy duplicates
            return;
        }
        Instance = this;
        externalFunctions = new InkExternalFunctions(); 
        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }

    public static DialogManager GetInstance()
    {
        return Instance; 
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        choisesText = new TextMeshProUGUI[choises.Length];
        int index = 0;
        foreach (GameObject choise in choises)
        {
            choisesText[index] = choise.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying) return;

        // Allow pressing E to continue or exit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (displayLineCoroutine != null)
            {
                // Skip the typing animation
                StopCoroutine(displayLineCoroutine);
                dialogueText.text = currentStory.currentText;
                canContinueToNextLine = true;
                displayLineCoroutine = null;
                DisplayChoises();
            }
            else if (canContinueToNextLine)
            {
                // Only continue if there are no active choices
                if (currentStory.canContinue &&
                    currentStory.currentChoices.Count == 0)
                {
                    ContinueStory();
                }
                // Exit only if no lines AND no choices
                else if (!currentStory.canContinue &&
                         currentStory.currentChoices.Count == 0)
                {
                    StartCoroutine(ExitDialogueMode());
                }
            }
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON,Animator emoteAnimator)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);
        externalFunctions.Bind(currentStory, emoteAnimator);

        

        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("right");

        ContinueStory(); 
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        dialogueVariables.StopListening(currentStory);
        externalFunctions.Unbind(currentStory);
   
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        Debug.Log("Dialogue exited."); // Debug confirmation

        StarterAssetsInputs playerInputs = FindAnyObjectByType<StarterAssetsInputs>();
        if (playerInputs != null)
        {
            playerInputs.jump = false; // Directly reset the jump flag
        }
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if(displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            if(currentStory.Continue().Equals("")&& !currentStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }
            else
            {
                HandleTags(currentStory.currentTags);
                displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
            }

            
            //DisplayChoises();

            
        }
        else
        {
            StartCoroutine(ExitDialogueMode()); // Exit if no more content
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = "";
        //dialogueText.maxVisibleCharacters = 0;

        bool isAddingRichTextTag = false;

        canContinueToNextLine = false;
        HideChoises();

        foreach (char letter in line)
        {
            if(letter=='<'||isAddingRichTextTag)
            {
                isAddingRichTextTag=true;
                dialogueText.text += letter;
                if (letter == '>')
                {
                    isAddingRichTextTag=false;
                }
            }
            else
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            
        }
        DisplayChoises();
        canContinueToNextLine = true;
    }

    private void HideChoises()
    {
        foreach (GameObject choiseButton in choises)
        {
            choiseButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (var tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: "+ tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch(tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                   
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag is read but is not curently being handled: " + tag);
                    break;
            }
        }
    }

    // For UI button
    public void OnClick_ContinueDialogue()
    {
        if (dialogueIsPlaying)
        {
            if (currentStory.canContinue)
            {
                ContinueStory();
            }
            else
            {
                StartCoroutine(ExitDialogueMode());
            }
        }
    }

    private void DisplayChoises()
    {
        List<Choice> currentChoises = currentStory.currentChoices;

        if (currentChoises.Count > choises.Length)
        {
            Debug.LogError("More choises were given that the UI can support. Number of choises given: " + currentChoises.Count);
        }
        int index = 0;
        foreach (Choice choice in currentChoises)
        {
            choises[index].gameObject.SetActive(true);
            choisesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choises.Length; i++)
        {
            choises[i].gameObject.SetActive(false);
        }
        StartCoroutine(SelectFirstChoice());

    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choises[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine&&choiceIndex >= 0 &&
        choiceIndex < currentStory.currentChoices.Count)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();   
        }
        
    }
    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if(variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }
    private void OnApplicationQuit()
    {
        dialogueVariables.SaveVariables();
    }
}
