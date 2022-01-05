using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemUIScript : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    Text itemDescription;
    [SerializeField]
    string description;
    
    public EventScriptableObject thisItemEvent;

    bool eventOccured = false;
    Image thisImage;


    // Start is called before the first frame update
    void Start()
    {
        CheckUnlock();

    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!eventOccured)
            return;
        itemDescription.text = description;
    }
    
    public void CheckUnlock()
    {
        if (thisItemEvent)
            eventOccured = GameManagerScript.Instance.occuredEvents.Contains(thisItemEvent.eventName);
        else
            eventOccured = true;
        thisImage = GetComponent<Image>();
        thisImage.enabled = eventOccured;
    }
}
