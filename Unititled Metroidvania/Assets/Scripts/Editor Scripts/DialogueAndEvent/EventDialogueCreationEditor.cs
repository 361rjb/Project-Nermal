using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EventDialogueCreationEditor : EditorWindow
{

    //Asset paths
    string eventAssetPath = "Assets/Objects/EventsAndDialogue/Event/";
    string dialogueAssetPath = "Assets/Objects/EventsAndDialogue/Dialogue/";

    //text styles
    GUIStyle leftStyle = new GUIStyle();
    
    GUIStyle nameStyle = new GUIStyle();
    GUIStyle dialogueStyle = new GUIStyle();

    //locations
    static Rect optionsArea = new Rect(0, 0, 300, 300);
    static Rect windowSize = new Rect(0, 0, 600, 300);
    Rect scrollBarPos = new Rect(10, 0, 30, 300);

    Rect newEventButtonArea = new Rect(0, 0, 50, 100);

    Vector2 scrollPos;
    Vector2 dialogueScrollPos;

    EventScriptableObject currentEditableEvent;



    //On editor window open
    [MenuItem("Window/DialogueEventCreator")]
    public static void ShowWindow()
    {
        EventDialogueCreationEditor window =  GetWindow<EventDialogueCreationEditor>("Event And Dialogue Creator");
        window.maxSize = windowSize.size;
        window.minSize = window.maxSize;
        
    }

    private void Awake()
    {

        leftStyle.richText = true;
        leftStyle.alignment = TextAnchor.MiddleLeft;

        dialogueStyle.richText = true;

        scrollBarPos.x = windowSize.width - scrollBarPos.x;
        scrollBarPos.height = windowSize.height;
        
    }

    

    // Start is called before the first frame update
    void OnGUI()
    {
        DrawOptions();

        DrawDialogueBoxes();

        Repaint();   
    }

    //Current State
    bool start = true;

    bool newEvent = false;
    bool editEventEntry = false;
    bool editEvent = false;

    void DrawOptions()
    {
       scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true,
            GUILayout.Width(optionsArea.width), GUILayout.Height(optionsArea.height));
        if (start)
        {
            if (GUILayout.Button("New Event"))
            {
                Debug.Log("New Event");
                start = false;
                currentEditableEvent = (EventScriptableObject)ScriptableObject.CreateInstance(typeof(EventScriptableObject)) ;
                
            }
            if (GUILayout.Button("Edit Event"))
            {
                Debug.Log("Edit Event");
                start = false;
                editEventEntry = true;
                currentEditableEvent = null;

            }
        }
        else
        {
            if (editEventEntry)
            {
                currentEditableEvent = (EventScriptableObject)EditorGUILayout.ObjectField(currentEditableEvent,
                    typeof(EventScriptableObject), false);
                if (currentEditableEvent != null)
                {
                    if (GUILayout.Button("Edit This Event"))
                    {
                        Debug.Log("Edit Event");
                        editEventEntry = false;
                        editEvent = true;

                    }
                }

            }
            else
            {
                DrawEventOptions();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                if (GUILayout.Button("Save Event"))
                {
                    Debug.Log("Save Event");
                    if (EditorUtility.DisplayDialog("Event Saving", "Are you sure you would like to save this event?",
                        "OK", "Cancel"))
                    {
                        if (AssetDatabase.LoadAssetAtPath(eventAssetPath + currentEditableEvent.eventName + ".asset", typeof(EventScriptableObject)) == null)
                        { AssetDatabase.CreateAsset(currentEditableEvent, eventAssetPath + currentEditableEvent.eventName + ".asset"); }
                        start = true;
                        editEvent = false;
                        currentEditableEvent = null;
                    }
                }
            }
        }
        GUILayout.EndScrollView();

    }

    //Dialogue variables
    DialogueObject currentDialogue;
    

    void DrawEventOptions()
    {
        GUILayout.Label("<color=#000000>\n Event Name</color>", leftStyle);
        currentEditableEvent.eventName = EditorGUILayout.TextField(currentEditableEvent.eventName);
        
        GUILayout.Label("<color=#000000>\n Can Occur Again?</color>", leftStyle);
        currentEditableEvent.canOccurAgain = EditorGUILayout.Toggle(currentEditableEvent.canOccurAgain);
        
        GUILayout.Label("<color=#000000>\n Ends With Boss?</color>", leftStyle);
        currentEditableEvent.isBoss = EditorGUILayout.Toggle(currentEditableEvent.isBoss);

        NewDialogueOptions();
    }


    void NewDialogueOptions()
    {
        if (currentDialogue == null)
        {


            if (GUILayout.Button("Add New Dialogue"))
            {
                Debug.Log("New Dialogue");
                currentDialogue = new DialogueObject();

            }
        }
        else
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Label("<color=#000000><size=20>\n New Dialogue</size></color>", leftStyle);

            GUILayout.Label("<color=#000000>\n Character Name</color>", leftStyle);
            currentDialogue.characterName = EditorGUILayout.TextField(currentDialogue.characterName);

            GUILayout.Label("<color=#000000>\n Is Player</color>", leftStyle);
            currentDialogue.isPlayer = EditorGUILayout.Toggle(currentDialogue.isPlayer);

            GUILayout.Label("<color=#000000>\n Font Text Size </color>", leftStyle);
            currentDialogue.fontSize = EditorGUILayout.IntField(currentDialogue.fontSize);
            
            EditorStyles.textField.wordWrap = true;

            GUILayout.Label("<color=#000000>\n Dialogue <b>UP TO 90 CHARACTERS</b></color>", leftStyle);
            currentDialogue.dialogueString = EditorGUILayout.TextArea(currentDialogue.dialogueString, GUILayout.Height(50));
                       
            GUILayout.Label("<color=#000000>\n Character Image</color>", leftStyle);
            currentDialogue.characterImage = (Sprite)EditorGUILayout.ObjectField(currentDialogue.characterImage, typeof(Sprite), false);



            if (GUILayout.Button("Save This Dialogue"))
            {
                Debug.Log("Saving Dialogue");
                if (!currentEditableEvent.dialogueEvents.Contains(currentDialogue))
                {
                    currentEditableEvent.dialogueEvents.Add(currentDialogue); 
                }
                currentDialogue = null;
                currentDialogueIndex = -1;

            }
        }
    }

    //DialogueViewVariables
    float yScrollPos= 0f;

    Rect dialogueBoxBase = new Rect(310, 0, 275, 60);

    Rect characterNamePosition = new Rect(5, 5, 100, 10);
    Rect dialogueTextPosition = new Rect(5, 17, 215, 50);
    Rect characterImagePosition = new Rect(5, 5, 50, 50);

    float inBetweenBoxSize = 5;

    Rect currentDrawBoxPos;
    Rect currentCharacterNamePos;
    Rect currentDialogueTextPos;
    Rect currentCharacterImagePos;

    Rect selectedDrawBox = new Rect(5, 5, 10, 10);
    Rect currentSelectedDrawBox;

    int count = 1;

    int currentDialogueIndex = -1;

    int lastClick = 0;

    void DrawDialogueBoxes()
    {
        if(currentEditableEvent == null)
        { currentDialogueIndex = -1; return; }


      
      

        currentDrawBoxPos.size = dialogueBoxBase.size;
        currentCharacterNamePos.size = characterNamePosition.size;
        currentDialogueTextPos.size = dialogueTextPosition.size;
        currentCharacterImagePos.size = characterImagePosition.size;
        count = 1;


        Event current = Event.current;

        foreach (DialogueObject dialogue in currentEditableEvent.dialogueEvents)
        {
        float baseY  = ((dialogueBoxBase.height + inBetweenBoxSize) * count) - yScrollPos;
            currentDrawBoxPos.x = dialogueBoxBase.x;
            currentDrawBoxPos.y = baseY;
            if (dialogue.isPlayer)
            {
                //image
                currentCharacterImagePos.x = dialogueBoxBase.x + characterImagePosition.x;
                currentCharacterImagePos.y = baseY + characterImagePosition.y;

                //CharacterName
                currentCharacterNamePos.x = dialogueBoxBase.x + currentCharacterImagePos.width + characterNamePosition.x;
                currentCharacterNamePos.y = baseY + characterNamePosition.y;

                currentDialogueTextPos.x = dialogueBoxBase.x + currentCharacterImagePos.width + dialogueTextPosition.x;
                currentDialogueTextPos.y = baseY + dialogueTextPosition.y;


            }
            else
            {
                //image
                currentCharacterImagePos.x = dialogueBoxBase.x + dialogueBoxBase.width - (characterImagePosition.x + characterImagePosition.width);
                currentCharacterImagePos.y = baseY + characterImagePosition.y;

                //CharacterName
                currentCharacterNamePos.x = dialogueBoxBase.x + dialogueBoxBase.width - ( characterImagePosition.width + characterNamePosition.x + characterNamePosition.width);
                currentCharacterNamePos.y = baseY + characterNamePosition.y;

                currentDialogueTextPos.x = dialogueBoxBase.x + dialogueBoxBase.width -( currentCharacterImagePos.width + dialogueTextPosition.width + dialogueTextPosition.x);
                currentDialogueTextPos.y = baseY + dialogueTextPosition.y;
            }

            if(currentDialogueIndex == currentEditableEvent.dialogueEvents.IndexOf(dialogue))
            {
                currentSelectedDrawBox.x = currentDrawBoxPos.x - selectedDrawBox.x;
                currentSelectedDrawBox.y = currentDrawBoxPos.y - selectedDrawBox.y;
                currentSelectedDrawBox.width = currentDrawBoxPos.width+ selectedDrawBox.width;
                currentSelectedDrawBox.height = currentDrawBoxPos.height + selectedDrawBox.height;
                EditorGUI.DrawRect(currentSelectedDrawBox, Color.black);
            }

            EditorGUI.DrawRect(currentDrawBoxPos, Color.white);

            EditorStyles.label.wordWrap = true;
            //Image
            if (dialogue.characterImage)
            {
                EditorGUI.DrawTextureTransparent(currentCharacterImagePos, dialogue.characterImage.texture);
            }
            //Name
            if(dialogue.isPlayer)
            {
                nameStyle.alignment = TextAnchor.UpperLeft;
            }
            else
            {
            
                nameStyle.alignment = TextAnchor.UpperRight;

            }

            EditorGUI.LabelField(currentCharacterNamePos, dialogue.characterName, nameStyle);
            
            //Text
            EditorGUI.LabelField(currentDialogueTextPos, "<size="+ dialogue.fontSize+">" + dialogue.dialogueString+"</size>", dialogueStyle);
            count++;

            

            if (currentDrawBoxPos.Contains(current.mousePosition) && current.button == 0 && current.isMouse )
            {
               
                    currentDialogueIndex = currentEditableEvent.dialogueEvents.IndexOf(dialogue);
            }


            if (currentDrawBoxPos.Contains(current.mousePosition) && current.type == EventType.ContextClick)
            {
                //Set Mouse Position and Create Menu
                GenericMenu menu = new GenericMenu();

                currentDialogueIndex = currentEditableEvent.dialogueEvents.IndexOf(dialogue);

                menu.AddDisabledItem(new GUIContent("Dialogue Options"));   //Whats Right clicked
               menu.AddItem(new GUIContent("Remove This Dialogue"), false, RemoveSelectedDialogue);  
                menu.AddItem(new GUIContent("Edit This Dialogue"), false, EditThisDialogue);    
              // menu.AddItem(new GUIContent("Move This Dialogue"), false, MoveThisDialogue);    
                menu.ShowAsContext();
                
            }

        }
       yScrollPos = GUI.VerticalSlider(scrollBarPos, yScrollPos, 0, currentEditableEvent.dialogueEvents.Count * (dialogueBoxBase.height + inBetweenBoxSize));
    }

    void RemoveSelectedDialogue()
    {
        currentEditableEvent.dialogueEvents.RemoveAt(currentDialogueIndex);
        currentDialogue = null;
        currentDialogueIndex = -1;
    }

    void EditThisDialogue()
    {
        currentDialogue = currentEditableEvent.dialogueEvents[currentDialogueIndex];
        GUI.FocusControl(null);
    }

}

