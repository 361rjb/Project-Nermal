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
    GameObject[] abilityEquiped = { null, null, null};
    string secondSlotUnlock;
    string thirdSlotUnlock;

    [SerializeField]
    Text abilityTitle;
    [SerializeField]
    Text abilityDescription;
    [SerializeField]
    Transform abilityArea;

    GameObject selectedAbilityGO;

    float cancelInput = 0;
    float lastCancelInput = 0;

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
    int currentSelectedSlot = 0;

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

                //equip abilites
                for (int i = 0; i < GameManagerScript.Instance.abilityEquiped.Length; i++)
                {
                    string s = GameManagerScript.Instance.abilityEquiped[i];
                    if (s != "-1")
                    {
                        abilityController.abilitySlots[i] = abilityScript;
                        GameObject abilityGO = (GameObject)Instantiate(ability);

                    }
                }
            }
        }
       
        selectedAbilityGO = null;
        currentSelectedAbility = 0;
        UpdateDisplay();

    }


    private void Update()
    {
        if(inAbilites)
        {
            GetInput();
            if (cancelInput == 1.0f && lastCancelInput != 1.0f)
            {
                eventSystem.SetSelectedGameObject(abilitySlot[currentSelectedSlot].gameObject);
               // if(selectedAbility) // NEED A WAY TO ENTER SELECTED ABILITY INTO SLOT AS WELL AS WHEN LEAVING THE MENU DESELECTING IT AS WELL
               // { selectedAbility = false; }
                inAbilites = false;
            }
            if (eventSystem.currentSelectedGameObject != selectedAbilityGO)
            {
                selectedAbilityGO = eventSystem.currentSelectedGameObject;

                UpdateDisplay();
            }
        }
    }

    void GetInput()
    {
        lastCancelInput = cancelInput;
        cancelInput = Input.GetAxisRaw("Cancel");

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
        abilitiesInMenu.Add(newAbility);
        iconScript.abilityScript = abilityScript;
        if (prevIcon)
        {
            prevIcon.SetDown(iconScript.buttonScript);
            iconScript.SetUp(iconScript.buttonScript);
        }
        prevIcon = iconScript;
        currentSpawn.y -= heightIncrement;
        iconScript.sprite.enabled = false;
        iconScript.title.enabled = false;
        selectedAbilityGO = newAbility;
        UpdateDisplay();
    }


    public void SetAbility(int slot)
    {
        currentSelectedSlot = slot;
        EnterAbilityScroll();
    }

    void EnterAbilityScroll()
    {
        if (abilitiesInMenu.Count > 0)
        {
            eventSystem.SetSelectedGameObject(abilitiesInMenu[currentSelectedAbility]);
            inAbilites = true;
        }
        else
        {
            currentSelectedSlot = -1;
        }
       // selectedEventGO = eventSystem.currentSelectedGameObject;
       // UpdateDisplay();
    }

    public void SelectAbility(AbilityUiIcon ability)
    {

        //Not working as intended there is another list with the abilities in PlayerAbilityController
        if(abilityEquiped[currentSelectedAbility] != null)
        {

            //Delete The ability if it is currently equiped aka unequip
            //{
            PlayerAbilityBase abilityEquipedScript = abilityEquiped[currentSelectedAbility].GetComponent<PlayerAbilityBase>();
            if (ability.abilityScript.eventUnlocksFrom.eventName == abilityEquipedScript.eventUnlocksFrom.eventName)
            {
                abilitySlot[currentSelectedSlot].image.sprite = null;
                abilityTitle.text = ability.abilityScript.title;
                abilityDescription.text = ability.abilityScript.description;
                eventSystem.SetSelectedGameObject(abilitySlot[currentSelectedSlot].gameObject);

                inAbilites = false;
                Destroy(abilityEquiped[currentSelectedAbility]);
                abilityController.abilitySlots[currentSelectedAbility] = null;
                return;
            }
            //}
            
            Destroy(abilityEquiped[currentSelectedAbility]);
                abilityController.abilitySlots[currentSelectedAbility] = null;

        }
        
        abilityTitle.text = ability.abilityScript.title;
        abilityDescription.text = ability.abilityScript.description;
        eventSystem.SetSelectedGameObject(abilitySlot[currentSelectedSlot].gameObject);
        
        GameObject newAbility = (GameObject)Instantiate(ability.abilityGO, abilityController.transform);
        abilityEquiped[currentSelectedAbility] = newAbility;
        abilityController.abilitySlots[currentSelectedAbility] = ability.abilityScript;
        inAbilites = false;

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
                currentSpawn.y -= heightIncrement;

            }
        }

    }

}
