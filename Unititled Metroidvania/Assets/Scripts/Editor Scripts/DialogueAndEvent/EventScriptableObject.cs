using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewEvent"), System.Serializable]
public class EventScriptableObject : ScriptableObject
{
    public List<DialogueObject> dialogueEvents = new List<DialogueObject>();

    public string eventName;

    //tests
    [Header ("Boolean Tests")]
    public bool canOccurAgain;
    public bool isBoss;

}
