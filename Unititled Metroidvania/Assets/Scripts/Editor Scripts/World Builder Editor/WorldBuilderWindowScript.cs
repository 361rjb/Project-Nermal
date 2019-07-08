using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Room
{
    public Vector2 pos;

    public List<Door> doors;


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


}

public struct Door
{
    public enum side { LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3 };
    public side doorSide;
    public Vector2 pos;
    public int roomNum;
}

public struct DoorConnection
{
    public Door connection;
    public Door thisDoor;
}

public class WorldBuilderWindowScript : EditorWindow
{

    int maxDoors = 3;

    Rect mapArea;
    

   static float hBarX = 100f;
   static float hBarY = 15f;
   static float hBarXEnd = -500f;
   
   static float vBarX = 10f;
   static float vBarY = 25f;
   static float vBarYEnd = -100f;
    
   static float zBarX = 100f;
   static float zBarY = 10f;
   
   static float defaultRoomX = 30f, defaultRoomY = 30f;
   static float defaultDoorX = 5f, defaultDoorY = 5f;
   

    List<Room> rooms = new List<Room>();

    int roomRightClicked = -1;
    int roomLeftClicked = -1;

    int doorRightClicked = -1;

    float xVal = 0, yVal = 0;
    float zVal = 1;
    Vector2 slider;

    float rectSizeX = 1000f, rectSizeY = 500f;

    static float optionsBoxWidth = 250f; 


    Vector2 clickPos;

    Color doorColor = Color.red;
    Color roomColor = Color.gray;
    Color optionsPanelColor = Color.black;
    Color lineColor = Color.blue;

    static Vector2 mapOrigin = new Vector2(hBarX + optionsBoxWidth, hBarY);


    [MenuItem("Window/WorldBuilder")]
    public static void ShowWindow()
    {
        GetWindow<WorldBuilderWindowScript>("WorldBuilder");

    }



    private void OnGUI()
    {


        Rect hBar = new Rect(hBarX + optionsBoxWidth, hBarY, position.width + hBarXEnd, 10.0f);
        Rect vBar = new Rect(vBarX + optionsBoxWidth, vBarY, 10f, position.height + vBarYEnd);
        Rect zBar = new Rect(zBarX + optionsBoxWidth, zBarY+ position.height + vBarYEnd, position.width + hBarXEnd, 10.0f);
        Rect optionsBox = new Rect(0, 0, optionsBoxWidth, position.height);

        mapArea = new Rect( hBarX + optionsBoxWidth, hBarY, position.width + hBarXEnd, position.height + vBarYEnd);

        EditorGUI.DrawRect(mapArea, Color.white);

        Event current = Event.current;

        if (mapArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
        {
            clickPos = current.mousePosition;
            GenericMenu menu = new GenericMenu();

            menu.AddDisabledItem(new GUIContent("Map Area"));
            menu.AddItem(new GUIContent("Create Room"), false, NewRoom);
            menu.ShowAsContext();

            //current.Use();
        }

        //Draw Any new rooms
        DrawRooms();
        
        xVal = GUI.HorizontalSlider(hBar, xVal, 0, -((mapArea.width * zVal) - mapArea.width));

        if(xVal > 0)
        {
            xVal = 0;
        }
        else if( xVal < -((mapArea.width * zVal) - mapArea.width))
        {
            xVal = -((mapArea.width * zVal) - mapArea.width);
        }

        yVal = GUI.VerticalSlider(vBar, yVal, 0,-((mapArea.height * zVal) - mapArea.height));

        if (yVal > 0)
        {
            yVal = 0;
        }
        else if (yVal < -((mapArea.height * zVal) - mapArea.height))
        {
            yVal = -((mapArea.height * zVal) - mapArea.height);
        }

        zVal = GUI.HorizontalSlider(zBar, zVal, 1.0f, 2.0f);


        slider = new Vector2(xVal, yVal);
       /* Handles.BeginGUI();
        Handles.color = Color.red;
        Handles.DrawLine(new Vector3(0, 0), new Vector3(300, 300));
        Handles.EndGUI();
        */
        //Draw Options Panel
        EditorGUI.DrawRect(optionsBox, optionsPanelColor);

    }

    void DrawRooms()
    {

        for (int i = 0; i < rooms.Count; i++)
        {

            //hBarX + optionsBoxWidth, hBarY

            Rect room = new Rect(((rooms[i].pos.x * zVal) + xVal)+mapOrigin.x, ((rooms[i].pos.y * zVal) + yVal) + mapOrigin.y, defaultRoomX * zVal, defaultRoomY * zVal);
            EditorGUI.DrawRect(room, roomColor);
            //Check room input

            Event current = Event.current;

            if (room.Contains(current.mousePosition) && current.type == EventType.ContextClick)
            {

                clickPos = current.mousePosition;
                roomRightClicked = i;
                GenericMenu menu = new GenericMenu();

                menu.AddDisabledItem(new GUIContent("Room"));
                menu.AddItem(new GUIContent("Remove Room"), false, RemoveRoom);
                menu.AddItem(new GUIContent("Add Door/Left"), false, AddDoor, Door.side.LEFT);
                menu.AddItem(new GUIContent("Add Door/Right"), false, AddDoor, Door.side.RIGHT);
                menu.AddItem(new GUIContent("Add Door/Up"), false, AddDoor, Door.side.UP);
                menu.AddItem(new GUIContent("Add Door/Down"), false, AddDoor, Door.side.DOWN);
                menu.ShowAsContext();

            }

            DisplayDoors(i);
        }
    }

    void DisplayDoors(int room)
    {
        Room thisRoom = rooms[room];

        float middleX = 0, middleY = 0;
        float halfSizeX = 0, halfSizeY = 0;
        float multiDoorX = 0, multiDoorY = 0;
        halfSizeX = (defaultRoomX * zVal / 2) - (defaultDoorX * zVal / 2);
        halfSizeY = (defaultRoomY * zVal / 2) - (defaultDoorY * zVal / 2);

        int leftDoorCount = 0, rightDoorCount = 0, downDoorCount = 0, upDoorCount = 0;

        for (int i = 0; i < thisRoom.doors.Count; i++)
        {

            middleX = halfSizeX + (thisRoom.pos.x * zVal) + xVal + mapOrigin.x;
            middleY = halfSizeY + (thisRoom.pos.y * zVal) + yVal + mapOrigin.y;

            Rect door = new Rect();
            switch (thisRoom.doors[i].doorSide)
            {
                case Door.side.LEFT:
                    {
                        leftDoorCount++;
                        multiDoorY = (defaultRoomY * zVal * ((float)leftDoorCount / ((float)thisRoom.leftDoors + 1))) - (defaultDoorY * zVal / 2);
                        middleY = multiDoorY + (thisRoom.pos.y * zVal) + yVal + mapOrigin.y;

                        door = new Rect(middleX - halfSizeX, middleY, defaultDoorX * zVal, defaultDoorY * zVal);

                        break;
                    }
                case Door.side.RIGHT:
                    {
                        rightDoorCount++;
                        multiDoorY = (defaultRoomY * zVal * ((float)rightDoorCount / ((float)thisRoom.rightDoors + 1))) - (defaultDoorY * zVal / 2);
                        middleY = multiDoorY + (thisRoom.pos.y * zVal) + yVal + mapOrigin.y;

                        door = new Rect(middleX + halfSizeX, middleY, defaultDoorX * zVal, defaultDoorY * zVal);

                        break;
                    }
                case Door.side.DOWN:

                    {
                        downDoorCount++;
                        multiDoorX = (defaultRoomX * zVal * ((float)downDoorCount / ((float)thisRoom.downDoors + 1))) - (defaultDoorX * zVal / 2);
                        middleX = multiDoorX + (thisRoom.pos.x * zVal) + xVal + mapOrigin.x;

                        door = new Rect(middleX, middleY + halfSizeY, defaultDoorX * zVal, defaultDoorY * zVal);

                        break;
                    }
                case Door.side.UP:
                    {
                        upDoorCount++;
                        multiDoorX = (defaultRoomX * zVal * ((float)upDoorCount / ((float)thisRoom.upDoors + 1))) - (defaultDoorX * zVal / 2);
                        middleX = multiDoorX + (thisRoom.pos.x * zVal) + xVal + mapOrigin.x;

                        door = new Rect(middleX, middleY - halfSizeY, defaultDoorX * zVal, defaultDoorY * zVal);
                        break;
                    }
                default:
                    break;
            }
            EditorGUI.DrawRect(door, doorColor);
            Event current = Event.current;

            if (door.Contains(current.mousePosition) && current.type == EventType.ContextClick)
            {
                clickPos = current.mousePosition;
                doorRightClicked = i;
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("Door"));
                menu.ShowAsContext();
                current.Use();
            }
        }
    }

    //Map Right click Menu Items

    void NewRoom()
    {

        //Fix Room Zom
        Room newRoom = new Room();
        newRoom.pos = ((clickPos/zVal) - (mapOrigin/zVal) - (slider / zVal));
        newRoom.doors = new List<Door>();
        newRoom.ResetDoors();

        rooms.Add(newRoom);
    }

    //Room Right click Menu Items

    void RemoveRoom()
    {
        rooms.RemoveAt(roomRightClicked);
    }

    void AddDoor(object sid)
    {

        Door nDoor = new Door();
        nDoor.doorSide = (Door.side)sid;
        nDoor.roomNum = roomRightClicked;
        rooms[roomRightClicked].doors.Add(nDoor);
        IncrementDoor(rooms[roomRightClicked], nDoor.doorSide, 1);
    }


    public void IncrementDoor(Room r, Door.side s, int ammount)
    {

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
    //
}