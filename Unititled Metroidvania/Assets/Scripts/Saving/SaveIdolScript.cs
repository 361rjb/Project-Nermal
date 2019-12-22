using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SaveIdolScript : Interactable
{
    PauseMenuInputScript uiHandler;
    public override void OnInteractEvent()
    {
        base.OnInteractEvent();
        Debug.Log("Interact to save");
        uiHandler.showSave = true;
        StartCoroutine(Save());
        //TO DO
        //move control to UI Character
    }
    // Start is called before the first frame update
   protected override void Start()
    {
        base.Start();
        uiHandler = GameObject.Find("UI Handler").GetComponent<PauseMenuInputScript>();

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
