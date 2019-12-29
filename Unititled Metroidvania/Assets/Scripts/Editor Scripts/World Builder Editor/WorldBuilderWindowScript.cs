using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/*
Know Issues**************************

+ Deleting and saving a connection in scriptable object doesnt work
+ Zooming in and Scrolling sets new position outside the white box array
+ Can't Loop through Dictionarys in for loop, switch to foreach. Look for Rooms.Count

*/

// Class that holds room data
[System.Serializable]
public class Room
{

    public string name = "newRoom";

    public Vector2 pos;

    public List<Door> doors;

    public int roomDictionaryID;

    public int leftDoors;
    public int rightDoors;
    public int upDoors;
    public int downDoors;

    public void ResetDoors()
    {
        leftDoors = 0;
        rightDoors = 0;
        upDoors = 0;
        downDoors = 0;
    }








    public bool thisRoomClickedLastFrame = false;

}

//class that holds door data
[System.Serializable]
public class Door
{
    public enum side { LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3 };
    public side doorSide;
    public Vector2 pos;
    public int roomNum;
    public DoorConnection connection;

    //Set position of door
    public void SetPos(Vector2 p) { pos = p; }
}


//Class for connections of doors
[System.Serializable]
public class DoorConnection
{
    public int connectionID;
    public Door connection;
    public Door thisDoor;
}

public class WorldBuilderWindowScript : EditorWindow
{
    //Size of white area   
    Rect mapArea;

    //Variables for scroll bars
    static float hBarX = 100f;
    static float hBarY = 15f;
    static float hBarXEnd = -500f;

    static float vBarX = 10f;
    static float vBarY = 25f;
    static float vBarYEnd = -100f;

    static float zBarX = 100f;
    static float zBarY = 10f;

    //Default room and door sizes
    static float defaultRoomX = 30f, defaultRoomY = 30f;
    static float defaultDoorX = 5f, defaultDoorY = 5f;

    //List of Rooms in the Level Editor
    Dictionary<int, Room> rooms = new Dictionary<int, Room>();

    //Currently Selected Room
    Room selectedRoom;


    //Connecting rooms Variables
    Door firstDoorSelected;
    bool connectingDoors;
    //All connections currently on screen
    List<DoorConnection> connections = new List<DoorConnection>();
    int currentNewKeyIndex = 0;

    //Room Variables
    int roomRightClicked = -1;

    //Door currently selected
    int doorRightClicked = -1;

    //Current values for the sliders
    float xVal = 0, yVal = 0;
    float zVal = 1;
    Vector2 slider;

    //box width for options
    static float optionsBoxWidth = 250f;


    //Selected Rooms options sizes
    Rect nameOptionBox = new Rect(120, 10, 100, 20);
    Rect textNamePos = new Rect(10, 10, 100, 20);
    Vector2 scrollPosition = new Vector2(0, 0);
    Rect labelAreaPostition = new Rect(10, 120, 10, 10);
    Rect labelAreaView = new Rect(10, 120, 10, 10);

    //Current clicked position
    Vector2 clickPos;

    //Default colors
    Color doorColor = Color.red;
    Color roomColor = Color.gray;
    Color optionsPanelColor = Color.black;
    Color lineColor = Color.blue;

    //Static for middle of map
    static Vector2 mapOrigin = new Vector2(hBarX + optionsBoxWidth, hBarY);

    //Create window
    [MenuItem("Window/WorldBuilder")]
    public static void ShowWindow()
    {
        GetWindow<WorldBuilderWindowScript>("WorldBuilder");

    }

    //When WorldBuilder Starts run following functions
    private void Awake()
    {
        LoadOnStart();
    }

    //Update and draw
    private void OnGUI()
    {

        //Scroll bar creation for Scrolling
        Rect hBar = new Rect(hBarX + optionsBoxWidth, hBarY, position.width + hBarXEnd, 10.0f);
        Rect vBar = new Rect(vBarX + optionsBoxWidth, vBarY, 10f, position.height + vBarYEnd);
        Rect zBar = new Rect(zBarX + optionsBoxWidth, zBarY + position.height + vBarYEnd, position.width + hBarXEnd, 10.0f);

        //Options box background Creation
        Rect optionsBox = new Rect(0, 0, optionsBoxWidth, position.height);

        //white Area behind the room visuals
        mapArea = new Rect(hBarX + optionsBoxWidth, hBarY, position.width + hBarXEnd, position.height + vBarYEnd);

        //Draw map backdrop
        EditorGUI.DrawRect(mapArea, Color.white);

        //Draw Options Panel
        EditorGUI.DrawRect(optionsBox, optionsPanelColor);
        //Draw Options for selected Room
        DrawOptions();

        //Create Event for new meu
        RightClickedOnMapArea();

        //Draw Any new rooms
        DrawRooms();

        //Map Area X sliders
        xVal = GUI.HorizontalSlider(hBar, xVal, 0, -((mapArea.width * zVal) - mapArea.width));

        //Max and min the X slider Value
        if (xVal > 0)
        {
            xVal = 0;
        }
        else if (xVal < -((mapArea.width * zVal) - mapArea.width))  //Zoomed Width max
        {
            xVal = -((mapArea.width * zVal) - mapArea.width);
        }

        //Map Area Y sliders
        yVal = GUI.VerticalSlider(vBar, yVal, 0, -((mapArea.height * zVal) - mapArea.height));

        //Max and min the Y slider Value
        if (yVal > 0)
        {
            yVal = 0;
        }
        else if (yVal < -((mapArea.height * zVal) - mapArea.height)) //Zoomed Height max
        {
            yVal = -((mapArea.height * zVal) - mapArea.height);
        }

        //Zoom Slider
        zVal = GUI.HorizontalSlider(zBar, zVal, 1.0f, 2.0f);

        //Slider position as Vector 2
        slider = new Vector2(xVal, yVal);

        //Input With Arrow keys for selected Room
        RoomInput();

        //Draw Line From Selected Door to mouse Until other Room is selected
        SelectConnectionAndDrawLine();

        //Draw all connections in layout 
        DrawLineConnections();

        //Redraw Frame
        Repaint();

    }


    //*****************************************************************************************************************************************
    //Newly Defined Functions Below
    //*****************************************************************************************************************************************

    //Right Click On White Area and show options
    void RightClickedOnMapArea()
    {

        Event current = Event.current;

        // If Right Clicked Create Menu
        if (mapArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
        {
            //Set Mouse Position and Create Menu
            clickPos = current.mousePosition;
            GenericMenu menu = new GenericMenu();

            menu.AddDisabledItem(new GUIContent("Map Area"));   //Whats Right clicked
            menu.AddItem(new GUIContent("Create Room"), false, NewRoom);    //Created a Room?
            menu.ShowAsContext();

        }

    }

    //Room Input For Arrow Keys and Move to New Position
    void RoomInput()
    {
        //Check if there is a room selected
        if (selectedRoom == null)
            return;

        //Arrow Inputs
        Event e = Event.current;
        if (e.isKey && e.keyCode == KeyCode.UpArrow)
        {
            selectedRoom.pos -= Vector2.up;
        }
        if (e.isKey && e.keyCode == KeyCode.DownArrow)
        {
            selectedRoom.pos += Vector2.up;
        }
        if (e.isKey && e.keyCode == KeyCode.LeftArrow)
        {
            selectedRoom.pos -= Vector2.right;
        }
        if (e.isKey && e.keyCode == KeyCode.RightArrow)
        {
            selectedRoom.pos += Vector2.right;
        }

        //Clamp positions to white area and zoomded area

        if (selectedRoom.pos.x < 0)
        {
            selectedRoom.pos.x = 0;

        }
        else if (selectedRoom.pos.x > mapArea.width - (defaultRoomX * zVal))
        {
            selectedRoom.pos.x = mapArea.width - (defaultRoomX * zVal);

        }

        if (selectedRoom.pos.y < 0)
        {
            selectedRoom.pos.y = 0;
        }
        else if (selectedRoom.pos.y > mapArea.height - (defaultRoomY * zVal))
        {
            selectedRoom.pos.y = mapArea.height - (defaultRoomY * zVal);
        }

    }

    //Draw all rooms in the list
    void DrawRooms()
    {
        //Draw Every room in Rooms
        foreach (KeyValuePair<int, Room> currentRoom in rooms)
        {
            //Draw Black Square behind room
            if (selectedRoom == currentRoom.Value)
            {
                //draw box behind selcted -5 
                Rect hovered = new Rect((((currentRoom.Value.pos.x - 5) * zVal) + xVal) + mapOrigin.x, (((currentRoom.Value.pos.y - 5) * zVal) + yVal) + mapOrigin.y, (defaultRoomX + 10) * zVal, (defaultRoomY + 10) * zVal);
                EditorGUI.DrawRect(hovered, Color.black);
            }


            //Current Room size and position values
            Rect room = new Rect(((currentRoom.Value.pos.x * zVal) + xVal) + mapOrigin.x, ((currentRoom.Value.pos.y * zVal) + yVal) + mapOrigin.y, defaultRoomX * zVal, defaultRoomY * zVal);

            //Draw Room
            EditorGUI.DrawRect(room, roomColor);

            //Check room mouse click
            Event current = Event.current;

            //Right Click Check
            if (room.Contains(current.mousePosition) && current.type == EventType.ContextClick)
            {
                //Set Mouse Position
                clickPos = current.mousePosition;
                //Set Room clicked
                roomRightClicked = currentRoom.Key;
                //Create Menu
                GenericMenu menu = new GenericMenu();

                //What did you Click on
                menu.AddDisabledItem(new GUIContent("Room"));
                //RemoveRoom Option
                menu.AddItem(new GUIContent("Remove Room"), false, RemoveRoom);

                //Add Door on side
                menu.AddItem(new GUIContent("Add Door/Left"), false, AddDoor, Door.side.LEFT);
                menu.AddItem(new GUIContent("Add Door/Right"), false, AddDoor, Door.side.RIGHT);
                menu.AddItem(new GUIContent("Add Door/Up"), false, AddDoor, Door.side.UP);
                menu.AddItem(new GUIContent("Add Door/Down"), false, AddDoor, Door.side.DOWN);
                menu.ShowAsContext();

            }

            //Left Click
            if (room.Contains(current.mousePosition) && current.button == 0 && current.isMouse)
            {
                //Set Selected Room
                selectedRoom = currentRoom.Value;
            }

            //Display all doors on the room
            DisplayDoors(currentRoom.Key);

        }
    }

    //Display Doors on Room
    void DisplayDoors(int room)
    {
        //Room that doors are drawing in
        Room thisRoom = rooms[room];

        //Position Values of Door
        float middleX = 0, middleY = 0; //Middle of Room
        float halfSizeX = 0, halfSizeY = 0; //HalfSize of Room
        float multiDoorX = 0, multiDoorY = 0; //Position if there are multiple doors on a side

        //Middle positions in half room size
        halfSizeX = (defaultRoomX * zVal / 2) - (defaultDoorX * zVal / 2);
        halfSizeY = (defaultRoomY * zVal / 2) - (defaultDoorY * zVal / 2);

        //Count Vars for doors on sides 
        int leftDoorCount = 0, rightDoorCount = 0, downDoorCount = 0, upDoorCount = 0;

        //For Each door in the room
        for (int i = 0; i < thisRoom.doors.Count; i++)
        {

            //Middle position plus the position of the room
            middleX = halfSizeX + (thisRoom.pos.x * zVal) + xVal + mapOrigin.x;
            middleY = halfSizeY + (thisRoom.pos.y * zVal) + yVal + mapOrigin.y;

            //new Var For door
            Rect door = new Rect();

            //Which Side is the Door on
            switch (thisRoom.doors[i].doorSide)
            {
                case Door.side.LEFT:
                    {
                        //Increment Count
                        leftDoorCount++;
                        //Check for All left side doors and move according to the position of them
                        multiDoorY = (defaultRoomY * zVal * ((float)leftDoorCount / ((float)thisRoom.leftDoors + 1))) - (defaultDoorY * zVal / 2);
                        //If door side count is 1 then the position will be in middle
                        middleY = multiDoorY + (thisRoom.pos.y * zVal) + yVal + mapOrigin.y;

                        //Set Door Var
                        door = new Rect(middleX - halfSizeX, middleY, defaultDoorX * zVal, defaultDoorY * zVal);

                        break;
                    }
                case Door.side.RIGHT:
                    {
                        //Increment Count
                        rightDoorCount++;
                        //Check for All right side doors and move according to the position of them
                        multiDoorY = (defaultRoomY * zVal * ((float)rightDoorCount / ((float)thisRoom.rightDoors + 1))) - (defaultDoorY * zVal / 2);
                        //If door side count is 1 then the position will be in middle
                        middleY = multiDoorY + (thisRoom.pos.y * zVal) + yVal + mapOrigin.y;

                        //Set Door Var
                        door = new Rect(middleX + halfSizeX, middleY, defaultDoorX * zVal, defaultDoorY * zVal);

                        break;
                    }
                case Door.side.DOWN:

                    {
                        //Increment Count
                        downDoorCount++;
                        //Check for All down side doors and move according to the position of them
                        multiDoorX = (defaultRoomX * zVal * ((float)downDoorCount / ((float)thisRoom.downDoors + 1))) - (defaultDoorX * zVal / 2);
                        //If door side count is 1 then the position will be in middle
                        middleX = multiDoorX + (thisRoom.pos.x * zVal) + xVal + mapOrigin.x;


                        //Set Door Var
                        door = new Rect(middleX, middleY + halfSizeY, defaultDoorX * zVal, defaultDoorY * zVal);

                        break;
                    }
                case Door.side.UP:
                    {
                        //Increment Count
                        upDoorCount++;
                        //Check for All down side doors and move according to the position of them
                        multiDoorX = (defaultRoomX * zVal * ((float)upDoorCount / ((float)thisRoom.upDoors + 1))) - (defaultDoorX * zVal / 2);
                        //If door side count is 1 then the position will be in middle
                        middleX = multiDoorX + (thisRoom.pos.x * zVal) + xVal + mapOrigin.x;


                        //Set Door Var
                        door = new Rect(middleX, middleY - halfSizeY, defaultDoorX * zVal, defaultDoorY * zVal);
                        break;
                    }
                default:
                    break;
            }

            //Draw new Room
            EditorGUI.DrawRect(door, doorColor);

            //Set Door position
            thisRoom.doors[i].SetPos(door.center);

            Event current = Event.current;

            //Check if the user is in the Create conncection state and trying to make a connection with the door with left click
            if (door.Contains(current.mousePosition) && connectingDoors && current.button == 0 && current.isMouse && firstDoorSelected != null && thisRoom.doors[i] != firstDoorSelected && thisRoom.doors[i].connection == null && thisRoom != rooms[firstDoorSelected.roomNum])
            {
                //Create Connection
                DoorConnection nConnection = new DoorConnection();
                //Set This door to the current door selected
                nConnection.thisDoor = firstDoorSelected;
                //Set connected door to the room and door at [i]
                nConnection.connection = thisRoom.doors[i];

                //Create the reverse connection
                DoorConnection nConnectionReverse = new DoorConnection();
                nConnectionReverse.connection = firstDoorSelected;
                nConnectionReverse.thisDoor = thisRoom.doors[i];

                //Set connections to rooms
                thisRoom.doors[i].connection = nConnectionReverse;
                firstDoorSelected.connection = nConnection;

                //Add connection to draw list
                connections.Add(nConnection);

                //remove current values selected
                connectingDoors = false;
                firstDoorSelected = null;
            }

            //Right click on Door
            if (door.Contains(current.mousePosition) && current.type == EventType.ContextClick)
            {
                //Set mouse position
                clickPos = current.mousePosition;
                //Current door Selected
                doorRightClicked = i;
                //Create Menu
                GenericMenu menu = new GenericMenu();

                //What is selected
                menu.AddDisabledItem(new GUIContent("Door"));

                //Delete Door
                menu.AddItem(new GUIContent("Remove Door"), false, RemoveDoor, thisRoom.doors[i]);

                //Check if there isn't connection to display an option to create one
                if (thisRoom.doors[i].connection == null)
                {
                    menu.AddItem(new GUIContent("Connect Room"), false, ConnectDoor, thisRoom.doors[i]);
                }

                //Check if there is a connection to display an option to delete one
                if (thisRoom.doors[i].connection != null)
                {
                    menu.AddItem(new GUIContent("Remove Connection"), false, RemoveConnection, thisRoom.doors[i]);
                }
                menu.ShowAsContext();
            }
        }
    }

    //Map Right click Menu Items********

    //Create New Room Button Option
    void NewRoom()
    {
        //Create Room
        Room newRoom = new Room();
        //Set postion according to zoom and slider
        newRoom.pos = ((clickPos / zVal) - (mapOrigin / zVal) - (slider / zVal));
        //Create empty list of doors
        newRoom.doors = new List<Door>();
        //Set Dictionary ID
        newRoom.roomDictionaryID = currentNewKeyIndex;
        //Reset Door values in room
        newRoom.ResetDoors();

        //Add to list and increment room key
        rooms.Add(currentNewKeyIndex, newRoom);
        currentNewKeyIndex++;
    }

    //Room Right click Menu Items****************

    //Remove Room from Map Area
    void RemoveRoom()
    {
        //Remove the selected room from memory
        if (selectedRoom == rooms[roomRightClicked])
        {
            selectedRoom = null;
        }

        //Remove All door connections in the room
        while (rooms[roomRightClicked].doors.Count > 0)
        {
            //Remove Connection From Room; Always 0 the size shrinks
            RemoveConnection(rooms[roomRightClicked].doors[0]);
            rooms[roomRightClicked].doors.RemoveAt(0);
        }
        //Remove Room from Dictionary
        rooms.Remove(roomRightClicked);
    }

    //Add Door with side 
    void AddDoor(object sid)
    {
        //New Door
        Door nDoor = new Door();
        //Set Side
        nDoor.doorSide = (Door.side)sid;
        //What Room is it in 
        nDoor.roomNum = roomRightClicked;
        //Add to List
        rooms[roomRightClicked].doors.Add(nDoor);
        //Increment Function adds one to side
        IncrementDoor(rooms[roomRightClicked], nDoor.doorSide, 1);

    }

    //Increments by amount ( can be negative ) to side
    public void IncrementDoor(Room r, Door.side s, int ammount)
    {
        //Which side is it on and Add to that amount
        switch (s)
        {
            case Door.side.LEFT:
                r.leftDoors += ammount;
                break;
            case Door.side.RIGHT:
                r.rightDoors += ammount;
                break;
            case Door.side.UP:
                r.upDoors += ammount;
                break;
            case Door.side.DOWN:
                r.downDoors += ammount;
                break;
            default:
                break;

        }
    }


    //Options Box

    //Draw options
    void DrawOptions()
    {
        //Don't Draw any options if there isn't anything selected
        if (selectedRoom != null)
        {
            //New GUI style with html and center flags
            GUIStyle style = new GUIStyle();
            style.richText = true;
            style.alignment = TextAnchor.MiddleCenter;

            //Set Scroll bar if the layout is large
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(optionsBoxWidth), GUILayout.Height(position.height));

            //Draw Label Name with text box to change name
            EditorGUI.LabelField(textNamePos, "<color=#ffffff>Name :</color>", style);
            selectedRoom.name = EditorGUI.TextField(nameOptionBox, selectedRoom.name);

            //Enter Twice
            GUILayout.Label("<color=#ffffff>\n\n </color>", style);

            //For Every connection write the room and connected room
            for (int c = 0; c < selectedRoom.doors.Count; c++)
            {
                if (selectedRoom.doors[c].connection != null)
                {
                    GUILayout.Label("<color=#ffffff>Room : " + rooms[selectedRoom.doors[c].connection.thisDoor.roomNum].name + "\nConnected Room : " + rooms[selectedRoom.doors[c].connection.connection.roomNum].name + "</color>", style);
                }
            }

            //Add new Style for buttons with html and button flags
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.richText = true;

            //If Save room button clicked Save Rooms
            if (GUILayout.Button("<color=#0000ff>Save Room Layouts</color>", buttonStyle))
            {
                SaveRoomLayouts();
                SaveRoomLayouts();
            }

            //If load Room as scene clicked. Load Scene in editor
            if (GUILayout.Button("<color=#0000ff>Load Room As Scene</color>", buttonStyle))
            {
                LoadRoomScene();
            
            }
            
            //Load Asset to side
            if (GUILayout.Button("<color=#0000ff>Show Scriptable object</color>", buttonStyle))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Levels/Level Objects/" + selectedRoom.name + ".asset");
                

            }


            //End Scroll Area
            GUILayout.EndScrollView();

        }

    }

    

    //Save all Rooms 
    void SaveRoomLayouts()
    {
        //Find Prefab in Asset Menu for Creating in scene later
        GameObject lvManagerPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Objects/Level Manager.prefab", typeof(GameObject));
        if (lvManagerPrefab == null)
        {
            //Error Finding Asset for level manager
            Debug.LogError("FAILED SAVING: Did not find levelManager prefab. Rename to \"Level Manager.prefab\" and move to \"Assets/Objects/\"");
            return;
        }

        //All the assets found in the Asset database with levelObj label
        List<string> roomsThatExist = new List<string>(AssetDatabase.FindAssets("l:LevelObj", new[] { "Assets/Levels/Level Objects" }));
        //Create an empty List for the Scriptable object that was found
        List<LevelObject> levelObjects = new List<LevelObject>();
        //All the roomNames as a list
        List<string> roomNames = new List<string>();

        //For every asset path found fill the empty list of scriptable objects with the found path
        foreach (string assetPathGUID in roomsThatExist)
        {
            //Asset path as string
            string assetPath = AssetDatabase.GUIDToAssetPath(assetPathGUID);
            //Load assetpath into a levelObject 
            LevelObject newLevel = (LevelObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(LevelObject));
            //if this is null it failed finding it and continue looking through asset paths
            if (newLevel == null)
            {
                continue;
            }
            //Add to list
            levelObjects.Add(newLevel);
            //add the name to the name list
            roomNames.Add(newLevel.name);
        }

        List<LevelObject> toDeleteList = new List<LevelObject>();
        //For every room that is in the scene Save to editor
        foreach (Room room in rooms.Values)
        {
            //If doesn't exist in the already loaded assets create new asset and scriptable object instance
            if (!roomNames.Contains(room.name))
            {
                //Create instance of a new Level
                LevelObject level = (LevelObject)ScriptableObject.CreateInstance(typeof(LevelObject));
                //Set name
                level.thisScene = room.name;

                //Set Editor position
                level.worldEditorX = room.pos.x;
                level.worldEditorY = room.pos.y;

                //Create empty list of connections
                level.doorways = new List<SceneConnector>();

                //For every Door in the room Add to empty connection list
                for (int d = 0; d < room.doors.Count; d++)
                {
                    //If the room doesnt have a connection continue
                    if (room.doors[d].connection != null)
                    {
                        //New Connection
                        SceneConnector newConnction = new SceneConnector();
                        //Set room connection to the connected door name
                        newConnction.connection = rooms[room.doors[d].connection.connection.roomNum].name;
                        //Set side of connection
                        newConnction.side = room.doors[d].doorSide;
                        //Set other side of the connection
                        newConnction.otherConnectionSide = room.doors[d].connection.connection.doorSide;
                        //Add to list
                        level.doorways.Add(newConnction);

                    }
                }

                //Create Asset in the Asset database with room name
                AssetDatabase.CreateAsset(level, "Assets/Levels/Level Objects/" + room.name + ".asset");
                //Label it so it can be found when saving again
                AssetDatabase.SetLabels(level, new[] { "LevelObj" });

                //Create scene and load it behind the editor
                Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                //set Scene Name to room name
                newScene.name = room.name;

                //Instantiate previously found prefab
                GameObject levelManager = Instantiate(lvManagerPrefab);
                //Set the level to the currently saved level
                levelManager.GetComponent<LevelManagerScript>().thisLevel = level;
                //Initiate values for room scene
                levelManager.GetComponent<LevelManagerScript>().CreateRoomSpecs();

                //Save scene to editor
                EditorSceneManager.SaveScene(newScene, "Assets/Levels/Level Scenes/" + room.name + ".unity");

                //Save open scene
                EditorSceneManager.SaveOpenScenes();
               
            }
            
            
            bool roomNotFound = true;
            //For every level that is already saved
            foreach (LevelObject level in levelObjects)
            {
                
                //  If level doesn't exist continue
                if (level == null)
                {
                    roomNotFound = true;
                                      
                    continue;
                }
                //if this room is the same as the level save updated features
                if (room.name == level.name)
                {
                    //Updated Positon
                    level.worldEditorX = room.pos.x;
                    level.worldEditorY = room.pos.y;

                    Scene newScene = EditorSceneManager.OpenScene("Assets/Levels/Level Scenes/" + room.name + ".unity"); ;
                    LevelManagerScript lvManager = GameObject.Find("Level Manager(Clone)").GetComponent<LevelManagerScript>();
                    lvManager.CreateRoomSpecs();

                    EditorSceneManager.SaveScene(newScene, "Assets/Levels/Level Scenes/" + room.name + ".unity");

                    //Create boolean for deleted
                    bool didntFindDoorInLevel = false;

                    //For every connection in the asset
                    for (int r = 0; r < level.doorways.Count; r++)
                    {
                        //for every door in the room
                        for (int d = 0; d < room.doors.Count; d++)
                        {
                            if(room.doors[d].connection == null)
                            {
                               // didntFindDoorInRoom = true;
                                continue;
                            }

                            if(rooms[room.doors[d].connection.connection.roomNum].name == level.doorways[r].connection)
                            {
                                didntFindDoorInLevel = false;
                                break;
                            }

                            if (rooms[room.doors[d].connection.connection.roomNum].name != level.doorways[r].connection)
                            {
                                didntFindDoorInLevel = true;
                                continue;
                            }
                            didntFindDoorInLevel = false;
                        }

                        //Remove doorway in asset if the connection is gone
                        if (didntFindDoorInLevel)
                        {
                            level.doorways.RemoveAt(r);
                        }
                    }

                    //Add door to scriptable object that doesnt exists yet
                    if (level.doorways.Count < rooms.Count)
                    {
                        //For every door that is in the room
                        foreach (Door oldDoor in room.doors)
                        {
                            //Create bool
                            bool notFound = false;
                            //for every connection in the level
                            foreach (SceneConnector levelObjectConnection in level.doorways)
                            {
                                //if connection already exists break else the room connection wasnt found
                                if (levelObjectConnection.connection == rooms[oldDoor.connection.connection.roomNum].name)
                                {
                                    notFound = false;
                                    break;
                                }
                                else
                                {
                                    notFound = true;
                                }
                            }

                            //not found add connection to asset
                            if (notFound)
                            {
                                //Create new connection
                                SceneConnector newConnction = new SceneConnector();
                                //Set connection to room name
                                newConnction.connection = rooms[oldDoor.connection.connection.roomNum].name;
                                //Set side
                                newConnction.side = oldDoor.doorSide;
                                //set other side
                                newConnction.otherConnectionSide = oldDoor.connection.connection.doorSide;
                                //Add connection to asset
                                level.doorways.Add(newConnction);
                            }
                        }
                    }
                    //Did find the room
                    roomNotFound = false;
                    
                }
                else 
                {
                    //Didnt find room add to list
                    roomNotFound = true;
                    toDeleteList.Add(level);
                }
                
            }
            
        }

        //Check the rooms to see if they were accidently added to the should remove list
        foreach(Room room in rooms.Values)
        {
            if (toDeleteList.Count >= 1)
            {
                
                foreach (LevelObject deletable in toDeleteList.ToArray())
                {
                    if (deletable != null && room != null)
                    {
                        //If it exists remove from list
                        if (deletable.name == room.name)
                        {
                            if (!toDeleteList.Remove(deletable))
                            {
                                Debug.Log("Didn't Delete");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Didn't Delete");
                    }
                }
            }
        }
        //Delete every room asset and scene in the Asset Database
        foreach (LevelObject delete in toDeleteList)
        {
            if (delete == null)
                continue;
            string deleteName = delete.name;
            AssetDatabase.DeleteAsset("Assets/Levels/Level Objects/" + deleteName + ".asset");
            AssetDatabase.DeleteAsset("Assets/Levels/Level Scenes/" + deleteName + ".unity");
        }
    }

    //Load Scene Currently selected
    void LoadRoomScene()
    {
        //Load selected scene
        EditorSceneManager.OpenScene("Assets/Levels/Level Scenes/" + selectedRoom.name + ".unity");
    }


    //Door Options ********************************************

    //Remove door from room
    void RemoveDoor(object door)
    {
        //Set door object parameter to door class
        Door rDoor = (Door)door;

        //Increment door by -1 to delete 
        IncrementDoor(rooms[rDoor.roomNum], rooms[rDoor.roomNum].doors[doorRightClicked].doorSide, -1);

        //if connection is null dont need to remove it from conection list otherwise remove it from connection list
        if (rDoor.connection != null)
        {
            RemoveConnection(rDoor.connection.thisDoor);
        }

        //Remove door from room list 
        rooms[rDoor.roomNum].doors.RemoveAt(doorRightClicked);

    }

    //Remove Connection
    void RemoveConnection(object door)
    {
        //Set door object parameter to door class
        Door thisDoor = (Door)door;

        //If there is no connection do nothing
        if (thisDoor.connection != null)
        {
            //Remove Connection and check if it worked 
            connections.Remove(thisDoor.connection);

            //Remove other side of the connection
            connections.Remove(thisDoor.connection.connection.connection);

            //Set to null
            thisDoor.connection.connection.connection = null;
            thisDoor.connection = null;
        }
    }

    //Set connecting start state and which the first door selected is, Option in door right click
    void ConnectDoor(object door)
    {
        connectingDoors = true;
        firstDoorSelected = (Door)door;
    }

    //Connections Drawing - Only draw When connecting rooms is true *******************************************
    void SelectConnectionAndDrawLine()
    {
        //Dont do anything if not in connecting doors state
        if (!connectingDoors)
        {
            return;
        }

        //if mouse clicked anywhere but door no longer draw line from door
        Event e = Event.current;
        if (e.button == 0 && e.isMouse)
        {
            connectingDoors = false;
        }

        //Draw line from mouse to door
        Handles.BeginGUI();
        Handles.color = Color.red;
        Handles.DrawLine(firstDoorSelected.pos, e.mousePosition);
        Handles.EndGUI();

    }

    //Draw all connections
    void DrawLineConnections()
    {

        Handles.BeginGUI();
        for (int i = 0; i < connections.Count; i++)
        {
            //Draw connection from door to other door for every connection
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(8, connections[i].thisDoor.pos, connections[i].connection.pos);
        }

        Handles.EndGUI();
    }

    //Load previously created rooms on start
    void LoadOnStart()
    {
        //Find all asset paths that already exist
        string[] levelNames = AssetDatabase.FindAssets("l:LevelObj", new[] { "Assets/Levels/Level Objects" });

        //Temporary list of asset objects
        List<LevelObject> tempList = new List<LevelObject>();

        //loop through found assets
        for (int i = 0; i < levelNames.Length; i++)
        {
            //Create new Level object with found path
            LevelObject newObject = (LevelObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelNames[i]), typeof(LevelObject));

            if (newObject == null) //check if newObject was a levelObject
            {

                continue;
            }

            //Add Object to temp list
            tempList.Add(newObject);

            //Create new room to hold data from asset
            Room newRoom = new Room();
            //set name
            newRoom.name = newObject.name;
            //set position from saved editor position
            newRoom.pos = new Vector2(newObject.worldEditorX, newObject.worldEditorY);
            //Create empty list of doorways
            newRoom.doors = new List<Door>();
            //Set Dictionary ID
            newRoom.roomDictionaryID = currentNewKeyIndex;
            //Reset door values to default
            newRoom.ResetDoors();
            //Add to room dictionary
            rooms.Add(currentNewKeyIndex, newRoom);
            //increment key
            currentNewKeyIndex++;

            //add doorways
            for (int d = 0; d < newObject.doorways.Count; d++)
            {
                //set room and add door
                roomRightClicked = i;
                //Add door to list 
                AddDoor(newObject.doorways[d].side);
                //Create new connection
                DoorConnection newConnection = new DoorConnection();
                //Set this door to connection
                newConnection.thisDoor = rooms[roomRightClicked].doors[d];
                //Set connection to door in list
                rooms[roomRightClicked].doors[d].connection = newConnection;
                //Add door to room foor list
            }
        }

        //Create Connections in for loop 
        //Increment for templist should be same size as rooms for every asset
        int r = 0;
        //for each room in the newly created dictionary
        foreach (Room room in rooms.Values)
        {
            //if there are doors in the asset add new connection
            if (tempList[r].doorways.Count >= 1)
            {

                //For Every door in room
                for (int d = 0; d < room.doors.Count; d++)
                {

                    //Find other door with connection
                    Door otherDoor = FindDoorWithConnection(room, tempList[r]);

                    //Continue if other didn't find the other door
                    if (otherDoor == null)
                    {
                        continue;
                    }
                    //Set connected door to other door
                    room.doors[d].connection.connection = otherDoor;
                    //Set other door connection to the current door looped through
                    otherDoor.connection.connection = room.doors[d];
                    //Add connection to list of connections
                    connections.Add(otherDoor.connection);

                }
            }
            //Increment to next tempList location
            r++;
        }

    }


    //Find Room with string
    /// <summary>
    /// This function does nothing
    /// </summary>
    /// <param name="name"> This parameter is of type <see cref="string"/> </param>
    /// <returns></returns>
    Room FindRoom(string name)
    {
        //Check every room in the room list
        foreach (Room room in rooms.Values)
        {
            //compare name to parameter name
            if (room.name == name)
            {
                //return room compared
                return room;
            }
        }
        //didn't find room with name
        return null;
    }

    //Get the door with the room connected
    Door FindDoorWithConnection(Room room, LevelObject level)
    {

        //Check each door
        foreach (Door door in room.doors)
        {
            //for each door connection in the level
            foreach (SceneConnector connection in level.doorways)
            {
                //Find room with connection name
                Room foundRoom = FindRoom(connection.connection);

                //Skip if room is null
                if (foundRoom == null)
                {
                    continue;
                }

                //If the doors room isn't the same as as the found room skip otherwise look for connection
                if (rooms[door.roomNum].name != foundRoom.name)
                {
                    //Check each door in other room
                    foreach (Door otherDoor in foundRoom.doors)
                    {
                        //if the connectios are already exist or do in the other door
                        if (otherDoor.connection.connection != null || otherDoor.connection == null)
                        {
                            continue;

                        }

                        //Check to see if these two doors have the other connections side and its side the same
                        if (otherDoor.doorSide == connection.otherConnectionSide)
                        {
                            //This is the other door in the connection
                            return otherDoor;
                        }
                    }
                }
            }
        }

        //Didnt find a door with that connection
        Debug.Log("returning null");
        return null;
    }

}

