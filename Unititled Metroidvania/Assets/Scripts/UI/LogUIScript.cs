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
    Vector2 currentSpawn;
    [SerializeField]
    float minY, maxY;

    List<LogEventScript> loggedEvents = new List<LogEventScript>();
    [Header("Be sure to add any new events that need\n to be logged to this list")]
    [Space]
    [SerializeField]
    List<EventScriptableObject> events = new List<EventScriptableObject>();

    [HideInInspector]
    public bool inLogs = false;
    float cancelInput = 0;
    float lastCancelInput = 0;
    float yInput = 0;
    float lastYInput = 0;

    int currentSelectedEvent = 0;
    GameObject selectedEventGO;

    [SerializeField]
    GameObject logButton;

    int occuredEventsLength = 0;
    LogEventScript prevEvent;

    // Start is called before the first frame update
    void Start()
    {

        prevEvent = null;
        currentSpawn = logSpawnPos;
        occuredEventsLength = GameManagerScript.Instance.occuredEvents.Count;
            foreach (string s in GameManagerScript.Instance.occuredEvents)
            {
        foreach(EventScriptableObject e in events)
        {
                if(e.eventName == s)
                {
                    NewEvent(s, e);                                       
                }
            }
        }

        selectedEventGO = loggedEvents[0].gameObject;
        currentSelectedEvent = 0;
        UpdateDisplay();
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
            if(eventSystem.currentSelectedGameObject != selectedEventGO)
            {
                selectedEventGO = eventSystem.currentSelectedGameObject;
               
                UpdateDisplay();
            }
            
        }

        int newCount = GameManagerScript.Instance.occuredEvents.Count;
        if (newCount > occuredEventsLength)
        {

            
            for (int i = occuredEventsLength; i < newCount; i++)
            {
                foreach (EventScriptableObject e in events)
                {
                    if (GameManagerScript.Instance.occuredEvents[i] == e.eventName)
                    {
                        NewEvent(GameManagerScript.Instance.occuredEvents[i], e);
                    }
                }         
            }

            occuredEventsLength = newCount;
        }
    }

    void GetInput()
    {
        lastCancelInput = cancelInput;
        cancelInput = Input.GetAxisRaw("Cancel");

        lastYInput = yInput;
        yInput= Input.GetAxisRaw("Vertical");

        
    }

    public void EnterLogScroll()
    {        
        eventSystem.SetSelectedGameObject(loggedEvents[currentSelectedEvent].gameObject);
        selectedEventGO = eventSystem.currentSelectedGameObject;
        inLogs = true;
        UpdateDisplay();
    }

    void NewEvent(string name, EventScriptableObject e)
    {
        GameObject newLog = (GameObject)Instantiate(logEventPrefab, logScrollBar.transform);
        newLog.name = name + " Log";
        newLog.transform.localPosition = currentSpawn;

        LogEventScript eventScript = newLog.GetComponent<LogEventScript>();
        eventScript.SetTitle(e.logTitle);
        eventScript.SetTextbox(e.loggedText, textBox);
        loggedEvents.Add(eventScript);
        if (prevEvent)
        {
            prevEvent.SetUp(eventScript.buttonScript);
            eventScript.SetDown(prevEvent.buttonScript);
        }
        prevEvent = eventScript;
        currentSpawn.y += heightIncrement;

        eventScript.sprite.enabled = false;
        eventScript.logText.enabled = false;

    }

    void UpdateDisplay()
    {
        if (selectedEventGO && selectedEventGO.GetComponent<LogEventScript>())
        {
        LogEventScript e;
        loggedEvents.ForEach(logevent => { logevent.sprite.enabled = false; logevent.logText.enabled = false; Debug.Log(logevent); }) ;
            e = selectedEventGO.GetComponent<LogEventScript>();
            int index = loggedEvents.IndexOf(e);
            currentSelectedEvent = index;
            currentSpawn = logSpawnPos;
            for (int i = -1; i <= 1; i++)
            {
                int j = index + i;
                if (j >= 0 && j < loggedEvents.Count)
                {
                    loggedEvents[j].transform.localPosition = currentSpawn;
                    loggedEvents[j].sprite.enabled = true;
                    loggedEvents[j].logText.enabled = true;
                }
                    currentSpawn.y += heightIncrement;

            }
        }

    }
}
