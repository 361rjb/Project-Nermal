using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : Interactable
{

    [SerializeField]
    EventScriptableObject thisDialogueEvent;

    PauseMenuInputScript uiHandler;

    [SerializeField]
    GameObject toEnableorDisable;
    [SerializeField]
    bool activate;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        uiHandler = GameObject.Find("UI Handler").GetComponent<PauseMenuInputScript>();
    }


    private void Update()
    {
        CheckReoccur();
    }
    public override void OnInteractEvent()
    {
        base.OnInteractEvent();
        uiHandler.StartDialogueEvent(thisDialogueEvent);
        if (toEnableorDisable)
        {
            toEnableorDisable.SetActive(!toEnableorDisable.activeSelf);
        }
    }

    void CheckReoccur()
    {
        if (thisDialogueEvent.canOccurAgain == false && GameManagerScript.Instance.occuredEvents.Contains(thisDialogueEvent.eventName))
        {
            if (toEnableorDisable)
            {
                toEnableorDisable.SetActive(activate);
            }
            gameObject.SetActive(false);
        }
    }
}
