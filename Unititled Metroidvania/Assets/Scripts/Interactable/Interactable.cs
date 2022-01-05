using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    protected PlayerControllerScript playerController;
    protected virtual void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerScript>();
    }
    public virtual void OnInteractEvent()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
                playerController.yInteract.SetActive(true);
            if (playerController.interactInput == 1.0f)
            {
                OnInteractEvent();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerController.yInteract.SetActive(false);
        }
    }
}
