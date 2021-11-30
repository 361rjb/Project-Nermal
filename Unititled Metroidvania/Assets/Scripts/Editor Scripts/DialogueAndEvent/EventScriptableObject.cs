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

    [Header("Log Data, Be Sure to add any event\n into LogUIScript in PauseMenu")]
    [Space]
    public string logTitle;
    [Space]
    [TextArea]
    public string loggedText;

    public GameObject abilityToUnlock;
}
