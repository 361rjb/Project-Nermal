using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManagerScript : MonoBehaviour
{
    [SerializeField]
    public LevelObject thisLevel;

    PlayerCamera cameraScript;
    PlayerControllerScript playerScript;

    [HideInInspector]
    public bool loadedLevel;

    public void CreateRoomSpecs()
    {
        CreateDoors();
    }

    // Start is called before the first frame update
    void Start()
    {
        loadedLevel = false;
        loadedLevel = LoadIntoLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool LoadIntoLevel()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<PlayerCamera>();
        if(cameraScript == null)
        {
            return false;
        }
        playerScript = GameObject.Find("player").GetComponent<PlayerControllerScript>();
        if(playerScript == null)
        {
            return false;
        }
        playerScript.SetLevel(this);
        
        SetCameraBounds();
        SetPosition();
        return true;
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
        for (int i = 0; i < thisLevel.doorways.Count; i++)
        {
            thisLevel.doorways[i].emptyDoor = new GameObject();

            //Collision
            thisLevel.doorways[i].doorway = thisLevel.doorways[i].emptyDoor.AddComponent<BoxCollider2D>();
            thisLevel.doorways[i].doorway.isTrigger = true;

            //Collision Box Size

            //Transform
            thisLevel.doorways[i].doorwayTransform = thisLevel.doorways[i].emptyDoor.transform;

            thisLevel.doorways[i].doorwayTransform.parent = transform;

            //Script Addition
            thisLevel.doorways[i].doorScript = thisLevel.doorways[i].emptyDoor.AddComponent<DoorwayScript>();



            //Connection 
            thisLevel.doorways[i].doorScript.SetRoomConnection(thisLevel.doorways[i].connection);
            
            thisLevel.doorways[i].doorScript.SetCurrentRoom(thisLevel.thisScene);
            thisLevel.doorways[i].doorScript.thisDoorSide = thisLevel.doorways[i].side;


            thisLevel.doorways[i].emptyDoor.name = thisLevel.thisScene + "_to_" + thisLevel.doorways[i].connection;
            thisLevel.doorways[i].emptyDoor.tag = "Doorway";

        }

    }

    void DestroyDoors()
    {
        for (int i = 0; i < thisLevel.doorways.Count; i++)
        {
            Destroy(thisLevel.doorways[i].emptyDoor);
        }
    }

}
