using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class GuardarJSON : MonoBehaviour
{
    [SerializeField]
    private ObjectsDatabaseSO objectsDatabase;
    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    private GameManager gameManager;
    
    [SerializeField]
    private PostProcessManager postProcessManager;
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private CheckpointSystem checkpointSystem;

    [SerializeField]
    private string saveDirectory = "Assets/Resources/"; //Application.dataPath

    public TMP_InputField fileName;

    Wrapper wrapper = new Wrapper();

    public void SaveToJson()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
        
        string filePath = Path.Combine(saveDirectory, fileName.text + ".json");

        wrapper.serializableObjects = new List<SerializableObject>(objectPlacer.serializableObjects.FindAll(track => track.ID != -1));
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadFromJson(string fileName)
    {
        objectPlacer.editmode = false;

        string filePath = Path.Combine(saveDirectory, fileName + ".json");

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        JsonUtility.FromJsonOverwrite(json, wrapper);

        Debug.Log(wrapper.serializableObjects.Count);
        foreach (var data in wrapper.serializableObjects)
        {
            var prefab = objectsDatabase.objectsData[data.ID].Prefab;
            int objectNumber = objectPlacer.PlaceObject(prefab, new Vector3Int(data.x, data.y, data.z), Quaternion.Euler(new Vector3(0, data.rotation, 0)), data.ID);

            if (data.ID == 7 || data.ID == 6)
            {
                if (data.ID == 6)
                {
                    GameObject car = objectPlacer.placedGameObjects[objectNumber].transform.Find("Arcade Car").gameObject;
                    GameObject startPos = objectPlacer.placedGameObjects[objectNumber].transform.Find("startPos").gameObject;
                    gameManager.SetupCar(car, startPos.transform);
                    levelManager.SetupCar(car);
                    postProcessManager.Setup(car);
                }
                checkpointSystem.RegisterCheckpoint(objectPlacer.placedGameObjects[objectNumber].GetComponentInChildren<Checkpoint>());
            }
        }

        checkpointSystem.InitializeCheckpoint();
    }
}


public class Wrapper
{
    public List<SerializableObject> serializableObjects;
}
