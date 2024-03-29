﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class IdolSavePoint
{
    public string roomName;
    public Vector3 location;
}

public class SaveIdolScript : Interactable
{
    IdolSavePoint thisSavePoint;

    PauseMenuInputScript uiHandler;
    public override void OnInteractEvent()
    {
        base.OnInteractEvent();
        if (uiHandler.showSave)
            return;
        Debug.Log("Interact to save");
        uiHandler.showSave = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerScript>().HealPlayer(-1);
        SaveLoadHandler.SaveGame(thisSavePoint);
        StartCoroutine(Save());
    }

    // Start is called before the first frame update
   protected override void Start()
    {
        base.Start();
        uiHandler = GameObject.Find("UI Handler").GetComponent<PauseMenuInputScript>();
        thisSavePoint = new IdolSavePoint();
        thisSavePoint.roomName = gameObject.scene.name;
        thisSavePoint.location = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Save()
    {
        yield return new WaitForSecondsRealtime(1f);
        uiHandler.showSave = false;
    }
}
