using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : Interactable
{

    [SerializeField]
    EventScriptableObject thisDialogueEvent;

    PauseMenuInputScript uiHandler;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        uiHandler = GameObject.Find("UI Handler").GetComponent<PauseMenuInputScript>();
    }

    public override void OnInteractEvent()
    {
        base.OnInteractEvent();
        uiHandler.StartDialogueEvent(thisDialogueEvent);
    }
}
