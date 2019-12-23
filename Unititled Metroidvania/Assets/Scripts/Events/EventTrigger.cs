using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    EventScriptableObject thisEvent;

    PauseMenuInputScript uiHandler;

    private void Start()
    {
        uiHandler = GameObject.Find("UI Handler").GetComponent<PauseMenuInputScript>();

        CheckReoccur();
    }

    private void Update()
    {
        CheckReoccur();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        uiHandler.StartDialogueEvent(thisEvent);
    }


    void CheckReoccur()
    {
        if (thisEvent.canOccurAgain == false && GameManagerScript.Instance.occuredEvents.Contains(thisEvent.eventName))
        {
            gameObject.SetActive(false);
        }
    }
}
