using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuInputScript : MonoBehaviour
{
    GameObject player;
    PlayerControllerScript playerController;
    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject savingImage;
    [Space]
    [Header("Dialogue Varibales")]


    [SerializeField]
    GameObject dialogueBox;

    [SerializeField]
    Text characterName;
    RectTransform characterNamePos;

    [SerializeField]
    Text dialogue;
    RectTransform dialoguePos; 

    [SerializeField]
    Image characterImage;
    RectTransform imagePos;

    Vector2 imageLeft = new Vector2(-110, 0); 
    Vector2 imageRight = new Vector2(110, 0); 


    float pauseInput;
    float lastPauseInput;

    float proceedDialogueInput;
    float lastProceedDialogueInput;

    bool inPause= false;

    public bool showSave = false;

    bool inDialogue = false;

    EventScriptableObject currentEvent = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerControllerScript>();
        characterNamePos = characterName.GetComponent<RectTransform>();
        dialoguePos = dialogue.GetComponent<RectTransform>();
        imagePos = characterImage.GetComponent<RectTransform>();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if(pauseInput == 1.0f && lastPauseInput != 1.0f && !showSave)
        {
            inPause = !inPause;
        }

        if (inPause || showSave || inDialogue)
        {
            Time.timeScale = 0.0f;
            if (showSave)
            {
                savingImage.SetActive(true);
            }
            else if (inPause)
            {
                pauseMenu.SetActive(true);
            }
            else if(inDialogue)
            {
                DrawDialogueEvent();
            }
            
            playerController.pausedGame = true;
        }
        else
        {
            Time.timeScale = 1.0f; 
            
                savingImage.SetActive(false);
            
                dialogueBox.SetActive(false);
                pauseMenu.SetActive(false);
            
            playerController.pausedGame = false;
        }
        
    }

    void GetInput()
    {
     
            
       if (inDialogue)
        {
        lastProceedDialogueInput = proceedDialogueInput;
        proceedDialogueInput= Input.GetAxisRaw("Submit");
        }
        else
        {
            lastPauseInput = pauseInput;
            pauseInput = Input.GetAxisRaw("Pause");
        }
    }

    public void OptionsButton()
    {
        Debug.Log("Show Options");
    }

    public void StartDialogueEvent(EventScriptableObject thisEvent)
    {
        playerController.pausedGame = true;
        inDialogue = true;
        dialogueBox.SetActive(true);
        eventDialogueIndex = 0;

        currentEvent = thisEvent;
    }

    Vector2 newNamePos = new Vector2();
    Vector2 newDialoguePos = new Vector2();

    int eventDialogueIndex = 0;
    void DrawDialogueEvent()
    {
        
        if (currentEvent.dialogueEvents[eventDialogueIndex].isPlayer)
        {
            characterName.alignment = TextAnchor.UpperLeft;
            imagePos.localPosition = imageLeft;
            newDialoguePos.x = 25;
            newDialoguePos.y = -5;
        }
        else
        {
            characterName.alignment = TextAnchor.UpperRight;
            imagePos.localPosition = imageRight;
            newDialoguePos.x = -25;
            newDialoguePos.y = -5;
        }
        dialoguePos.localPosition = newDialoguePos;
        
        

            characterName.text = currentEvent.dialogueEvents[eventDialogueIndex].characterName;

            dialogue.text = "<size=" + currentEvent.dialogueEvents[eventDialogueIndex].fontSize + ">" +
                currentEvent.dialogueEvents[eventDialogueIndex].dialogueString + "</size>";

            characterImage.sprite = currentEvent.dialogueEvents[eventDialogueIndex].characterImage;

        if (lastProceedDialogueInput != 1.0f && proceedDialogueInput == 1.0f)
        {
            eventDialogueIndex++;
        }
        if(eventDialogueIndex >= currentEvent.dialogueEvents.Count)
        {
            eventDialogueIndex = -1;
            inDialogue = false;
            dialogueBox.SetActive(false);
            if (!currentEvent.canOccurAgain)
            {
                GameManagerScript.Instance.occuredEvents.Add(currentEvent.eventName);
            }
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exiting");
    }

}
