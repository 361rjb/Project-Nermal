using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogUIScript : MonoBehaviour
{
    [SerializeField]
    Text logText;
    [SerializeField]
    Text textBox;


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
        
    }

    public void EnterLogScroll()
    {

    }
}
