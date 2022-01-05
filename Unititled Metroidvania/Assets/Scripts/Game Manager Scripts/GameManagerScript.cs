using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class KeyItemState
{
    public string itemName;
    public bool state;
    public KeyItemState(string s , bool b)
    {
        state = b;
        itemName = s;
    }
}
public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;
    [SerializeField]
    string playerScene;

    [SerializeField]
    string testLoadScene;

    [SerializeField]
    string testPauseUI;


    [SerializeField]
    string mainMenu;

    public Vector2 playerStartPos;

    //Loaded Variables
    public List<string> occuredEvents = new List<string>();
    public int currentMaxHealth;
    public int collectedHealthContainers;

    public int gameIndex;

    //AbilitesUnlocked
    public List<string> keyItems = new List<string>();

    public List<KeyItemState> keyItemStates = new List<KeyItemState>();


    public string[] abilityEquiped = { "-1", "-1", "-1" }; // if -1 slot is locked

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach(string s in keyItems)
        {
            keyItemStates.Add(new KeyItemState(s, false));
        }

        SceneManager.LoadScene(mainMenu, LoadSceneMode.Additive);
        //Temporary Load Scene
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckState(string s)
    {
        foreach (KeyItemState ki in keyItemStates)
        {
            if (ki.itemName == s)
            {
                return ki.state;
            }
        }
        return false;
    }

    public void UnlockItem(string item)
    {
        foreach(KeyItemState ki in keyItemStates)
        {
            if(ki.itemName == item)
            {
                ki.state = true;
            }
        }
            
    }

    public void Load(int index)
    {
        gameIndex = index;
        if (SceneManager.GetSceneByName(mainMenu).isLoaded)
        {
            SceneManager.UnloadSceneAsync(mainMenu);
        }
        SceneManager.LoadScene(playerScene, LoadSceneMode.Additive);
        SceneManager.LoadScene(testPauseUI, LoadSceneMode.Additive);
       PlayerSave thisSave = SaveLoadHandler.LoadGame(gameIndex);
        if (thisSave == null)
        {
            Debug.Log("Room Not or First Save");
            SceneManager.LoadScene(testLoadScene, LoadSceneMode.Additive);
            occuredEvents = new List<string>();
            playerStartPos = Vector2.zero;
            currentMaxHealth = 6;
            collectedHealthContainers = 0;

            keyItemStates = new List<KeyItemState>();
            foreach (string s in keyItems)
            {
                keyItemStates.Add(new KeyItemState(s, false));
            }
        }
        else
        {
            Debug.Log("Loading at room " + thisSave.lastSavePoint.roomName);

            SceneManager.LoadScene(thisSave.lastSavePoint.roomName, LoadSceneMode.Additive);
            playerStartPos = thisSave.lastSavePoint.location;
            occuredEvents = thisSave.eventsTriggered;
            currentMaxHealth = thisSave.currentMaxHealth;
            collectedHealthContainers = thisSave.collectedHealthContainers;
            abilityEquiped = thisSave.abilityEquiped;
            keyItemStates = new List<KeyItemState>();
            foreach (KeyItemState s in thisSave.keyItems)
            {
                keyItemStates.Add(s);
            }
        }

    }

    public void Reload(string currentLevel)
    {        

        SceneManager.UnloadSceneAsync(playerScene);
        SceneManager.UnloadSceneAsync(currentLevel);
        SceneManager.UnloadSceneAsync(testPauseUI);

        Load(gameIndex);
    }

    public void MainMenu(string currentLevel)
    {

        SceneManager.UnloadSceneAsync(playerScene);
        SceneManager.UnloadSceneAsync(currentLevel);
        SceneManager.UnloadSceneAsync(testPauseUI);

        SceneManager.LoadScene(mainMenu, LoadSceneMode.Additive);
    }

}
