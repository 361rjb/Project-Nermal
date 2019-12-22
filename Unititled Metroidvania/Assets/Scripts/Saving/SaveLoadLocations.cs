using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SaveLocations")]
public class SaveLoadLocations : ScriptableObject
{

    public List<IdolSavePoint> SaveLocations;

}

[System.Serializable]
public class IdolSavePoint
{
    public string roomName;
    public Vector3 location;
}