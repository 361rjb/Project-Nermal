using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class PlayerSave
{
    public int saveIndex;

    //location
    public IdolSavePoint lastSavePoint;

    //unimplemented
    public int currency;

    public int currentMaxHealth;

    // Abilities
    public List<KeyItemState> keyItems;

    public List<string> eventsTriggered;

    public string[] abilityEquiped = {"-1", "-1", "-1"}; // if -1 slot is locked
}

public class SaveLoadHandler
{
    

    public static void SaveGame(IdolSavePoint savePoint)
    {

        string savePath = "PlayerSave" + GameManagerScript.Instance.gameIndex + ".json";
        PlayerSave newSave = new PlayerSave();

        newSave.saveIndex = GameManagerScript.Instance.gameIndex;
        newSave.lastSavePoint = savePoint;
        newSave.eventsTriggered = GameManagerScript.Instance.occuredEvents;
        newSave.currentMaxHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerScript>().currentMaxHealth;
        newSave.keyItems = new List<KeyItemState>();
        newSave.abilityEquiped = GameManagerScript.Instance.abilityEquiped;
        foreach (KeyItemState s in GameManagerScript.Instance.keyItemStates)
        {
            newSave.keyItems.Add(s);
        }
        string filePath = Path.Combine(Application.dataPath, savePath);

        string toJSon = JsonUtility.ToJson(newSave);
        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, toJSon);
        }
        else
        {
            File.WriteAllText(filePath, toJSon);
        }
        
    }

    public static PlayerSave LoadGame(int index)
    {
        string savePath = "PlayerSave" + index + ".json";
        string filePath = Path.Combine(Application.dataPath, savePath);
        string json = "";

        if (File.Exists(filePath))
        {
            json = File.ReadAllText(filePath);
        }
        PlayerSave fromJson = JsonUtility.FromJson<PlayerSave>(json);
        return fromJson;
    }

}
