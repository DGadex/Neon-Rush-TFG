using UnityEngine;
using System.Collections.Generic;
public class GuardarJSON : MonoBehaviour
{
    [SerializeField]
    private ObjectsDatabaseSO objectsDatabase;
    [SerializeField]
    private ObjectPlacer objectPlacer;

    string filePath = "Assets/Resources/gridData.json";

    Wrapper wrapper = new Wrapper();

    public void SaveToJson()
    {
        wrapper.serializableObjects = new List<SerializableObject>(objectPlacer.serializableObjects);
        string json = JsonUtility.ToJson(wrapper, true);
        System.IO.File.WriteAllText(filePath, json);
    }

    public void LoadFromJson()
    {
        string json = System.IO.File.ReadAllText(filePath);
        JsonUtility.FromJsonOverwrite(json, wrapper);
        Debug.Log(wrapper.serializableObjects.Count);
        foreach (var data in wrapper.serializableObjects)
        {
            var prefab = objectsDatabase.objectsData[data.ID].Prefab;
            objectPlacer.PlaceObject(prefab, new Vector3Int(data.x, data.y, data.z), Quaternion.Euler(new Vector3 (0, data.rotation, 0)), data.ID);
        }
    }
}


public class Wrapper
{
    public List<SerializableObject> serializableObjects;
}
