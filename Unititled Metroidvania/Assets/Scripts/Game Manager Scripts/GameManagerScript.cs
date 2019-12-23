using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;
    [SerializeField]
    string playerScene;

    [SerializeField]
    string testLoadScene;

    [SerializeField]
    string testPauseUI;

    public Vector2 playerStartPos;


    public List<string> occuredEvents = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //Temporary Load Scene
        TestingLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void TestingLoad()
    {
        SceneManager.LoadScene(testPauseUI, LoadSceneMode.Additive);
        SceneManager.LoadScene(playerScene, LoadSceneMode.Additive);
       PlayerSave thisSave = SaveLoadHandler.LoadGame();
        if (thisSave == null)
        {
            Debug.Log("Room Not or First Save");
            SceneManager.LoadScene(testLoadScene, LoadSceneMode.Additive); 
        }
        else
        {
            Debug.Log("Loading at room " + thisSave.lastSavePoint.roomName);

            SceneManager.LoadScene(thisSave.lastSavePoint.roomName, LoadSceneMode.Additive);
            playerStartPos = thisSave.lastSavePoint.location;
            occuredEvents = thisSave.eventsTriggered;
        }

    }

}
