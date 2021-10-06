using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuInputScript : MonoBehaviour
{
    public static PauseMenuInputScript Instance;

    GameObject player;
    PlayerControllerScript playerController;
    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject savingImage;
    [Space]
    [Header("Dialogue Varibales")]

    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    LogUIScript logSystem;

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

    [SerializeField]
    Image boxImage;

    [SerializeField]
    Sprite playerBoxSprite;

    [SerializeField]
    Sprite otherBoxSprite;

    [Space]
    [SerializeField]
    Sprite healthImage;
    
    [SerializeField]
    Sprite healthImageDamaged;

    [SerializeField]
    GameObject healthPrefab;

    //Menu Tabs variables
    [Header("Menu Tabs"), SerializeField]
    List<GameObject> menuContentHolders = new List<GameObject>();

    [SerializeField]
    List<Button> menuTabs = new List<Button>();
    [SerializeField]
    List<Selectable> firstSelectableInTab = new List<Selectable>();

    [SerializeField]
    List<Vector2> tabsAnchors = new List<Vector2>();

    int currentTabIndex = 2;

    Vector2 imageLeft = new Vector2(-110, 0);
    Vector2 imageRight = new Vector2(110, 0);


    float pauseInput;
    float lastPauseInput;
    float lastCancelInput;
    float cancelInput;

    float proceedDialogueInput;
    float lastProceedDialogueInput;

    bool inPause = false;

    public bool showSave = false;

    bool inDialogue = false;

    EventScriptableObject currentEvent = null;

    List<Image> healthIcons = new List<Image>();

    int healthIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

        if(!Instance)
        {
            Instance = this;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerControllerScript>();
        characterNamePos = characterName.GetComponent<RectTransform>();
        dialoguePos = dialogue.GetComponent<RectTransform>();
        imagePos = characterImage.GetComponent<RectTransform>();
        Vector2 spawnPos = new Vector2(-200, 130);

        for(int i = 0; i < ((float)playerController.currentMaxHealth)/2; i++)
        {
            GameObject healthIcon = (GameObject)Instantiate(healthPrefab, dialogueBox.transform.parent);
            healthIcon.transform.localPosition = spawnPos;
            spawnPos.x += 35;
            healthIcons.Add(healthIcon.GetComponent<Image>());
        }
        healthIndex = playerController.currentMaxHealth;

    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (pauseInput == 1.0f && lastPauseInput != 1.0f && !showSave)
        {            
            inPause = !inPause;
            
            eventSystem.SetSelectedGameObject(menuTabs[currentTabIndex].gameObject);
        }
        if(inPause && !logSystem.inLogs && (cancelInput == 1.0f && lastCancelInput != 1.0f && pauseInput != 1.0f))
        {
            inPause = false;
        }


        if (inPause || showSave || inDialogue)
        {
            if (showSave)
            {
                Time.timeScale = 0.0f;
                savingImage.SetActive(true);
            }
            else if (inPause)
            {
                Time.timeScale = 0.0f;
                pauseMenu.SetActive(true);
                
            }
            else if (inDialogue)
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
            proceedDialogueInput = Input.GetAxisRaw("Submit");
        }
        else
        {
            lastPauseInput = pauseInput;
            pauseInput = Input.GetAxisRaw("Pause");
            lastCancelInput = cancelInput;
            cancelInput = Input.GetAxisRaw("Cancel");

        }
    }


    public void StartDialogueEvent(EventScriptableObject thisEvent)
    {
        playerController.pausedGame = true;
        inDialogue = true;
        dialogueBox.SetActive(true);
        eventDialogueIndex = 0;

        currentEvent = thisEvent;
        PlayEventAnimation();
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
            boxImage.sprite = playerBoxSprite;
        }
        else
        {
            characterName.alignment = TextAnchor.UpperRight;
            imagePos.localPosition = imageRight;
            newDialoguePos.x = -25;
            newDialoguePos.y = -5;
            boxImage.sprite = otherBoxSprite;
        }
        dialoguePos.localPosition = newDialoguePos;



        characterName.text = currentEvent.dialogueEvents[eventDialogueIndex].characterName;

        dialogue.text = "<size=" + currentEvent.dialogueEvents[eventDialogueIndex].fontSize + ">" +
            currentEvent.dialogueEvents[eventDialogueIndex].dialogueString + "</size>";

        characterImage.sprite = currentEvent.dialogueEvents[eventDialogueIndex].characterImage;

        if (lastProceedDialogueInput != 1.0f && proceedDialogueInput == 1.0f)
        {
            if (gameObjectAnimated)
            {
                if (!gameObjectAnimated.isPlaying)
                {
                    eventDialogueIndex++;
                    if (eventDialogueIndex >= currentEvent.dialogueEvents.Count)
                    {
                        eventDialogueIndex = -1;

                        if (currentEvent.isBoss)
                        {
                            BossRoomManagerScript.Instance.EnableBoss();
                        }
                        inDialogue = false;
                        dialogueBox.SetActive(false);
                        if (!currentEvent.canOccurAgain)

                        {
                            GameManagerScript.Instance.occuredEvents.Add(currentEvent.eventName);
                        }
                    }
                    else
                    {

                        PlayEventAnimation();
                    }
                }
            }
            else
            {
                eventDialogueIndex++;
                if (eventDialogueIndex >= currentEvent.dialogueEvents.Count)
                {
                    eventDialogueIndex = -1;

                    if (currentEvent.isBoss)
                    {
                        BossRoomManagerScript.Instance.EnableBoss();
                    }

                    inDialogue = false;
                    dialogueBox.SetActive(false);

                    if (!currentEvent.canOccurAgain)

                    {
                        GameManagerScript.Instance.occuredEvents.Add(currentEvent.eventName);
                    }

                }
                else
                {

                    PlayEventAnimation();
                }
            }
        }
    }

    Animation gameObjectAnimated;
    void PlayEventAnimation()
    {
        if (currentEvent.dialogueEvents[eventDialogueIndex].clip)
        {
            currentEvent.dialogueEvents[eventDialogueIndex].clip.legacy = true;
            gameObjectAnimated = GameObject.Find(currentEvent.dialogueEvents[eventDialogueIndex].gameobjectNameThatPlaysClip).GetComponent<Animation>();
            gameObjectAnimated.AddClip(currentEvent.dialogueEvents[eventDialogueIndex].clip, currentEvent.dialogueEvents[eventDialogueIndex].clip.name);

            Debug.Log(gameObjectAnimated.GetClipCount());
            gameObjectAnimated.Play(currentEvent.dialogueEvents[eventDialogueIndex].clip.name);
        }
    }


    public void PlayerTakeDamage(int amount)
    {
        StartCoroutine(TickHealth  (amount, true));
    }

    public void PlayerHeal(int amount)
    {
        StartCoroutine(TickHealth(amount, false));
    }

    IEnumerator TickHealth(int amount, bool isDamage)
    {
        for(float i = 0; i < amount; i++)
        {
            if(isDamage && healthIndex - i > 0)
            {
                int index = Mathf.Clamp(Mathf.CeilToInt(((float)healthIndex - i) / 2f), 0, healthIcons.Count);
                Debug.Log("Index " + index);
                if ((healthIndex - i) % 2 == 0 )
                {
                    healthIcons[index-1].sprite = healthImageDamaged;
                }
                else 
                {
                    healthIcons[index-1].enabled = false;
                }

            }
            else if (i + healthIndex <= healthIcons.Count*2)
            {

                int index = Mathf.Clamp(Mathf.CeilToInt(((float)healthIndex + i) / 2f), 0, healthIcons.Count);
                if ((healthIndex + i) % 2f == 1)
                {
                    healthIcons[index-1].enabled = true;
                    healthIcons[index-1].sprite = healthImageDamaged;

                }
                else
                {
                    healthIcons[index-1].enabled = true;
                    healthIcons[index-1].sprite = healthImage;
                }
            }

            yield return new WaitForSecondsRealtime(0.3f);
        }

        if (isDamage)
        {
            healthIndex = Mathf.Clamp(healthIndex - amount , 0, playerController.currentMaxHealth);
        }
        else
        {

            healthIndex = Mathf.Clamp(healthIndex + amount , 0, playerController.currentMaxHealth);
        }
        Debug.Log("HealthIndex " + healthIndex);
    }


//**********************************************************
//Buttons Scripts
//********************************************************** 


    public void OpenTab(int tabIndex)
    {
        menuContentHolders.ForEach(h => h.SetActive(false));
        menuContentHolders[tabIndex].SetActive(true);
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;
        int tabsCount = menuTabs.Count;
        for(int i =0, j = tabIndex; i < tabsCount; ++i, ++j)
        {
            if (j >= tabsCount)
            {
                j = 0;
            }

            menuTabs[j].transform.localPosition =  tabsAnchors[i];
            newNav.selectOnLeft = menuTabs[j].navigation.selectOnLeft;
            newNav.selectOnRight= menuTabs[j].navigation.selectOnRight;
            newNav.selectOnDown = firstSelectableInTab[tabIndex];
            menuTabs[j].navigation = newNav;

        }



        currentTabIndex = tabIndex;


    }

    public void ExitGame()
    {
        Debug.Log("Exiting");
        GameManagerScript.Instance.MainMenu(playerController.currentLevelScript.thisLevel.thisScene);
    }
    
    public void CreditsButton()
    {
        Debug.Log("Show Credits");
    }

    public void SoundsButton()
    {
        Debug.Log("Show Sounds");
    }
    public void ControlsButton()
    {
        Debug.Log("Show Controls");
    }
}