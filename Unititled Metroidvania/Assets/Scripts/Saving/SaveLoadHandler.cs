using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class PlayerSave
{
    //location
    public IdolSavePoint lastSavePoint;
    
    //unimplemented
    public int currency;
    
    // Abilities
    public bool dash;
    public bool wallJump;
    public bool attack;

    public List<string> eventsTriggered;
}

public class SaveLoadHandler
{
    

    public static void SaveGame(IdolSavePoint savePoint)
    {

        string savePath = "PlayerSave.json";
        PlayerSave newSave = new PlayerSave();
        newSave.lastSavePoint = savePoint;
        newSave.eventsTriggered = GameManagerScript.Instance.occuredEvents;
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

    public static PlayerSave LoadGame()
    {
        string savePath = "PlayerSave.json";
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
