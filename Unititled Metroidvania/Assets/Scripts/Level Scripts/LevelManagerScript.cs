using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManagerScript : MonoBehaviour
{
    [SerializeField] 
    LevelObject thisLevel;

    PlayerCamera cameraScript;
    PlayerControllerScript playerScript;

    [HideInInspector]
    public bool loadedLevel;


    // Start is called before the first frame update
    void Start()
    {
        loadedLevel = false;
        LoadIntoLevel();
        loadedLevel = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadIntoLevel()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<PlayerCamera>();
        playerScript = GameObject.Find("player").GetComponent<PlayerControllerScript>();
        playerScript.SetLevel(this);
        CreateDoors();
        SetCameraBounds();
        SetPosition();
    }


    void OnDestroy()
    {
        DestroyDoors();
    }

    void SetPosition()
    {
        transform.position = new Vector3(thisLevel.xOffset, thisLevel.yOffset);
    }

    void SetCameraBounds()
    {
        cameraScript.SetLevel(thisLevel);
    }
       

    void CreateDoors()
    {
        for (int i = 0; i < thisLevel.doorways.Length; i++)
        {
            thisLevel.doorways[i].emptyDoor = new GameObject();

            //Collision
            thisLevel.doorways[i].doorway = thisLevel.doorways[i].emptyDoor.AddComponent<BoxCollider2D>();
            thisLevel.doorways[i].doorway.isTrigger = true;
            thisLevel.doorways[i].doorway.size = thisLevel.doorways[i].colliderSize;

            //Collision Box Size

            //Transform
            thisLevel.doorways[i].doorwayTransform = thisLevel.doorways[i].emptyDoor.transform;
            thisLevel.doorways[i].doorwayTransform.position = thisLevel.doorways[i].position;

            thisLevel.doorways[i].doorwayTransform.parent = transform;

            //Script Addition
            thisLevel.doorways[i].doorScript = thisLevel.doorways[i].emptyDoor.AddComponent<DoorwayScript>();



            //Connection 
            thisLevel.doorways[i].doorScript.SetRoomConnection(thisLevel.doorways[i].connection);
            thisLevel.doorways[i].doorScript.SetCurrentRoom(thisLevel.thisScene);

            thisLevel.doorways[i].emptyDoor.name = thisLevel.thisScene + "_to_" + thisLevel.doorways[i].connection;

        }

    }

    void DestroyDoors()
    {
        for (int i = 0; i < thisLevel.doorways.Length; i++)
        {
            Destroy(thisLevel.doorways[i].emptyDoor);
        }
    }

}
