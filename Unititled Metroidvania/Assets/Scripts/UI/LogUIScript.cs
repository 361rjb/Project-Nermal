using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LogUIScript : MonoBehaviour
{
    [SerializeField]
    Text textBox;

    [SerializeField]
    EventSystem eventSystem;

    [SerializeField]
    GameObject logEventPrefab;

    [SerializeField]
    Scrollbar logScrollBar;

    [SerializeField]
    Vector2 logSpawnPos;
    [SerializeField]
    float heightIncrement;
    [SerializeField]
    float minY, maxY;

    List<LogEventScript> loggedEvents = new List<LogEventScript>();
    [Header("Be sure to add any new events that need\n to be logged to this list")]
    [Space]
    [SerializeField]
    List<EventScriptableObject> events = new List<EventScriptableObject>();


    bool inLogs = false;
    float cancelInput = 0;
    float lastCancelInput = 0;

    [SerializeField]
    GameObject logButton;

    // Start is called before the first frame update
    void Start()
    {

        LogEventScript prevEvent = null;
        Vector2 currentSpawn= logSpawnPos;
            foreach(string s in GameManagerScript.Instance.occuredEvents)
            {
        foreach(EventScriptableObject e in events)
        {
                if(e.eventName == s)
                {
                    GameObject newLog = (GameObject)Instantiate(logEventPrefab, logScrollBar.transform);
                    newLog.name = s + " Log";
                    newLog.transform.localPosition = currentSpawn;

                    LogEventScript eventScript = newLog.GetComponent<LogEventScript>();
                    eventScript.SetTitle(e.logTitle);
                    eventScript.SetTextbox(e.loggedText, textBox);
                    loggedEvents.Add(eventScript);
                    if(prevEvent)
                    {
                        prevEvent.SetUp(eventScript.buttonScript);
                        eventScript.SetDown(prevEvent.buttonScript);
                    }
                    prevEvent = eventScript;
                    currentSpawn.y += heightIncrement;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inLogs)
        {
            GetInput();

            if(cancelInput == 1.0f && lastCancelInput != 1.0f)
            {
                eventSystem.SetSelectedGameObject(logButton.gameObject);
                inLogs = false;
            }
        }
    }

    void GetInput()
    {
        lastCancelInput = cancelInput;
        cancelInput = Input.GetAxisRaw("Cancel");
    }

    public void EnterLogScroll()
    {
        eventSystem.SetSelectedGameObject(loggedEvents[0].gameObject);
        inLogs = true;
    }
}
