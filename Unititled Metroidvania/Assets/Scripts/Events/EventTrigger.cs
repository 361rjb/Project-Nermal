using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    EventScriptableObject thisEvent;

    PauseMenuInputScript uiHandler;

    [SerializeField]
    float delay = 0;

    [SerializeField]
    GameObject toEnableorDisable;
    [SerializeField]
    bool activate = false;

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
        if (collision.tag == "Player")
        {
                            StartCoroutine(DelayEvent());
            
        }
    }

    IEnumerator DelayEvent()
    {
        yield return new WaitForSecondsRealtime(delay);
            uiHandler.StartDialogueEvent(thisEvent);
        if (toEnableorDisable)
        {
            toEnableorDisable.SetActive(activate);
        }

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
