using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthContainer : Interactable
{
    const string HEALTH_STRING = "HealthContainer";


    public int healthContainerID = 0;

    protected override void Start()
    {
        base.Start();
        if( GameManagerScript.Instance.occuredEvents.Contains(HEALTH_STRING+healthContainerID))
        {
            gameObject.SetActive(false);
        }
    }

    public override void OnInteractEvent()
    {
        playerController.CollectHealthContainer();
        GameManagerScript.Instance.occuredEvents.Add(HEALTH_STRING + healthContainerID);
        gameObject.SetActive(false);
    }
}
