using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using System;
using UnityEngine.EventSystems;


public class DialogueManager : MonoBehaviour
{
  
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;

  /*  [Header("Global Ink FIle")]
    [SerializeField] private InkFile globalsInkFile;*/

    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;

    [Header("Choice UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;
    private Animator layoutAnimator;
    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    private bool canContinueNextLine = false;

    private bool isAddRichText = false;
    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private DialogueVariables dialogueVariables;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Acha mais de um Dialogue na Cena");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        //get layoutanimator
        layoutAnimator =dialoguePanel.GetComponent<Animator>();

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        if( canContinueNextLine &&
            currentStory.currentChoices.Count == 0 && InputManager.GetInstance().GetInteractPressed())
        {
            ContinueStory();
        }
    }
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);
        //reset layout, portrait, speaker

        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("rigth");

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if(displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
           displayLineCoroutine= StartCoroutine(DisplayLine(currentStory.Continue()));

           

            HandleTags(currentStory.currentTags);
        }
        else
        {
          StartCoroutine(ExitDialogueMode());

        }

    }

    private IEnumerator DisplayLine (string line)
    {
        // texto vazio
        dialogueText.text = "";

        continueIcon.SetActive(false);
        HideChoices();

        canContinueNextLine = false;


        foreach (char letter in line.ToCharArray())        
        {

            if (InputManager.GetInstance().GetSubmitPressed())
            {
                dialogueText.text = line;
                break;
            }

            if(letter == '<' || isAddRichText)
            {
                isAddRichText = true;
                dialogueText.text += letter;
                if(letter == '>')
                {
                    isAddRichText = false;
                }
            }
            else
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            
        }

        continueIcon.SetActive(true);

        DisplayChoices();

        canContinueNextLine = true;
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(":");
            if(splitTag.Length != 2)
            {
                Debug.LogError("não pode ter menos que 2" + tag);
            }

            string tagKey = splitTag[0];
            string tagValue = splitTag[1];

            //handle the tag

            switch (tagKey)
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
                    Debug.LogWarning("Erro na manipulação da tag" + tag);
                    break;
            }
        }
    }
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length)
        {
            Debug.LogError("Numero de escolha maior que o permitido na UI" + currentChoices);
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            InputManager.GetInstance().RegisterSubmitPressed();
            ContinueStory();
        }
        
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);

        if (variableValue == null)
        {
            Debug.LogWarning("ink variable foi achada como null" + variableName);
        }

        return variableValue;
    }
}
