using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField]
    string playerScene;

    [SerializeField]
    string testLoadScene;

    [SerializeField]
    string testPauseUI;

    // Start is called before the first frame update
    void Start()
    {
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
        SceneManager.LoadScene(testLoadScene, LoadSceneMode.Additive);

    }

}
