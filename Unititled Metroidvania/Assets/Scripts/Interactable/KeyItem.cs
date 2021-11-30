using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Interactable
{
    [SerializeField]
    string itemName;
    // Start is called before the first frame update
    [SerializeField]
    EventScriptableObject thisEvent;

    PauseMenuInputScript uiHandler;

    [SerializeField]
    GameObject toEnableorDisable;
    [SerializeField]
    bool activate = false;

    protected override void Start()
    {
        base.Start();

        uiHandler = GameObject.Find("UI Handler").GetComponent<PauseMenuInputScript>();
        CheckReoccur();
    }

    private void Update()
    {
        CheckReoccur();
    }

    // Interact with the key item
    public override void OnInteractEvent()
    {
        base.OnInteractEvent(); 
        if (toEnableorDisable)
        {
            toEnableorDisable.SetActive(activate);
        }
        GameManagerScript.Instance.UnlockItem(itemName);
        uiHandler.StartDialogueEvent(thisEvent);
        
    }

    void CheckReoccur()
    {
        if (thisEvent.canOccurAgain == false && GameManagerScript.Instance.occuredEvents.Contains(thisEvent.eventName))
        {
            if (toEnableorDisable)
            {
                toEnableorDisable.SetActive(activate);
            }
            gameObject.SetActive(false);
        }
    }
}
