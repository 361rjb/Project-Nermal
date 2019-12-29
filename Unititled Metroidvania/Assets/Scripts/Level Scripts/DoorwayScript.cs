using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class DoorwayScript : MonoBehaviour
{
    [SerializeField] string connectedRoom;
    [SerializeField] string currentRoom;

    PlayerControllerScript playerScript;

    Vector2 newPlayerPosition = new Vector2();

    public Door.side thisDoorSide;

    public bool isInteractable = false;

    void Start()
    {
        playerScript = GameObject.Find("player").GetComponent<PlayerControllerScript>();
    }

    public void SetRoomConnection(string room)
    {
        connectedRoom = room;
    }

    public void SetCurrentRoom(string room)
    {
        currentRoom = room;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!isInteractable)
            {

                StartCoroutine(TransitionPause());
                playerScript.LevelTransition();

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isInteractable)
        {

            playerScript.yInteract.SetActive(true);
            if (playerScript.interactInput == 1.0f)
            {
                StartCoroutine(TransitionPause());
                playerScript.LevelTransition();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerScript.yInteract.SetActive(false);
        }
    }


    IEnumerator TransitionPause()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(connectedRoom, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(currentRoom);
        playerScript.SetPlayerPositionWithDoors(currentRoom, connectedRoom);

    }
}
