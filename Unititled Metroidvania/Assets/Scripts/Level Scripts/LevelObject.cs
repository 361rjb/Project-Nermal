using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level Object")]
public class LevelObject : ScriptableObject
{
    public string thisScene; //Use Object instead???
    [Space]
    public SceneConnector[] doorways;

    [Header("Camera Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Space]
    [Header("Level Offsets")]    
    public float xOffset;
    public float yOffset;
    
   

    
}



[System.Serializable]
public struct SceneConnector
{
    [HideInInspector]
    public GameObject emptyDoor;

    public Vector2 position;

    public string connection;

    [HideInInspector]
    public BoxCollider2D doorway;

    [HideInInspector]
    public Transform doorwayTransform; //AddComponent<>()

    [HideInInspector]
    public DoorwayScript doorScript;

    public Vector2 colliderSize;
}
