using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityUIScript : MonoBehaviour
{
    [SerializeField]
    EventSystem eventSystem;
    bool inAbilites;
    bool selectedAbility;

    //Ability Slots
    [SerializeField]
    List<Selectable> abilitySlot = new List<Selectable>();
    string secondSlotUnlock;
    string thirdSlotUnlock;

    [SerializeField]
    Text abilityTitle;
    [SerializeField]
    Text abilityDescription;
    [SerializeField]
    Selectable abilityArea;

    GameObject selectedAbilityGO;

    float cancelInput = 0;
    float lastCancelInput = 0;
    float yInput = 0;
    float lastYInput = 0;

    [SerializeField]
    GameObject abilityIconPrefab;
    AbilityUiIcon prevIcon;
    [SerializeField]
    Vector2 logSpawnPos;
    [SerializeField]
    float heightIncrement;
    Vector2 currentSpawn;
    [SerializeField]
    float minY, maxY;

    int currentSelectedAbility = 0;

    [SerializeField]
    List<GameObject> playerAbilities = new List<GameObject>();
    
    List<AbilityUiIcon> unlockedPlayerAbilities = new List<AbilityUiIcon>();
    List<GameObject> abilitiesInMenu = new List<GameObject>();

    PlayerAbilityController abilityController;

    private void Start()
    {
        prevIcon = null;
        abilityController = GameObject.FindObjectOfType<PlayerAbilityController>();
        foreach(GameObject ability in playerAbilities)
        {
            PlayerAbilityBase abilityScript = ability.GetComponent<PlayerAbilityBase>();
            //Check the events to see if the player has unlocked the ability
            if(GameManagerScript.Instance.occuredEvents.Contains( abilityScript.eventUnlocksFrom.eventName ))
            {
                UnlockAbilty(ability);
            }
        }

        currentSelectedAbility = 0;

    }


    private void Update()
    {
        if(inAbilites)
        {
            GetInput();
            if (cancelInput == 1.0f && lastCancelInput != 1.0f)
            {
                eventSystem.SetSelectedGameObject(abilityArea.gameObject);
                if(selectedAbility) // NEED A WAY TO ENTER SELECTED ABILITY INTO SLOT AS WELL AS WHEN LEAVING THE MENU DESELECTING IT AS WELL
                { selectedAbility = false; }
                inAbilites = false;
            }
        }
    }

    void GetInput()
    {
        lastCancelInput = cancelInput;
        cancelInput = Input.GetAxisRaw("Cancel");

        lastYInput = yInput;
        yInput = Input.GetAxisRaw("Vertical");
    }


    public void UnlockAbilty(GameObject ability)
    {
        PlayerAbilityBase abilityScript = ability.GetComponent<PlayerAbilityBase>();
        //Add new icon to abiltiy menu
        GameObject newAbility = (GameObject)Instantiate(abilityIconPrefab, abilityArea.transform);
        AbilityUiIcon iconScript = newAbility.GetComponent<AbilityUiIcon>();
        newAbility.name = abilityScript.title + "_ability";
        newAbility.transform.localPosition = currentSpawn;
        unlockedPlayerAbilities.Add(iconScript);

        //LogEventScript eventScript = newLog.GetComponent<LogEventScript>();
        //eventScript.SetTitle(e.logTitle);
        //eventScript.SetTextbox(e.loggedText, textBox);
        //loggedEvents.Add(eventScript);
        iconScript.ability = abilityScript;
        if (prevIcon)
        {
            prevIcon.SetUp(iconScript.buttonScript);
            iconScript.SetDown(iconScript.buttonScript);
        }
        prevIcon = iconScript;
        currentSpawn.y += heightIncrement;
        iconScript.sprite.enabled = false;
        iconScript.title.enabled = false;
    }


    public void SetAbility(int slot)
    {

    }

    public void EnterAbilityScroll()
    {
        if (abilitiesInMenu.Count > 0)
        {
            eventSystem.SetSelectedGameObject(abilitiesInMenu[currentSelectedAbility]);
        }
       // selectedEventGO = eventSystem.currentSelectedGameObject;
        inAbilites = true;
       // UpdateDisplay();
    }

    public void SelectAbility(PlayerAbilityBase ability)
    {
        abilityTitle.text = ability.title;
        abilityDescription.text = ability.description;
        eventSystem.SetSelectedGameObject(abilitySlot[0].gameObject);
    }
    void UpdateDisplay()
    {
        if (selectedAbilityGO && selectedAbilityGO.GetComponent<AbilityUiIcon>())
        {
            AbilityUiIcon e;
            unlockedPlayerAbilities.ForEach(abilityIcon => { abilityIcon.sprite.enabled = false; abilityIcon.title.enabled = false; Debug.Log(abilityIcon); });
            e = selectedAbilityGO.GetComponent<AbilityUiIcon>();
            int index = unlockedPlayerAbilities.IndexOf(e);
            currentSelectedAbility = index;
            currentSpawn = logSpawnPos;
            for (int i = -1; i <= 1; i++)
            {
                int j = index + i;
                if (j >= 0 && j < unlockedPlayerAbilities.Count)
                {
                    unlockedPlayerAbilities[j].transform.localPosition = currentSpawn;
                    unlockedPlayerAbilities[j].sprite.enabled = true;
                    unlockedPlayerAbilities[j].title.enabled = true;
                }
                currentSpawn.y += heightIncrement;

            }
        }

    }

}
