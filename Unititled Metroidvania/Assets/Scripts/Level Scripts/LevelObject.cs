using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level Object")]
public class LevelObject : ScriptableObject
{
    public string thisScene; //Use Object instead???
    [Space]
    public List<SceneConnector> doorways;

    [Header("Camera Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Space]
    [Header("Level Offsets")]    
    public float xOffset;
    public float yOffset;

    [HideInInspector]
    public float worldEditorX;
    [HideInInspector]
    public float worldEditorY;

    [Space]
    [Header("Player Must interact to traverse")]
    public bool isInteractable = false;

}



[System.Serializable]
public class SceneConnector
{
    [HideInInspector]
    public GameObject emptyDoor;

    public string connection;

    public Door.side side;
    public Door.side otherConnectionSide;

    [HideInInspector]
    public BoxCollider2D doorway;

    [HideInInspector]
    public Transform doorwayTransform; //AddComponent<>()

    [HideInInspector]
    public DoorwayScript doorScript;
    
}
