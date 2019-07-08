using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class DoorwayScript : MonoBehaviour
{
    string connectedRoom;
    string currentRoom;

    PlayerControllerScript playerScript;

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
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.LevelTransition();
            StartCoroutine(TransitionPause());
        }
    }


    IEnumerator TransitionPause()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(connectedRoom, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(currentRoom);
    }
}
