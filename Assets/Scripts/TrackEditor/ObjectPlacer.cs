using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> placedGameObjects = new();

    
    [SerializeField]
    public List<SerializableObject> serializableObjects = new();

    [SerializeField]
    private Button metaButton;

    [SerializeField]
    private InputManager inputManager;

    public bool editmode = true;

    public bool escenarioProbado = true;

    public bool hayMeta = false;

    public bool hayCheckpoint = false;

    private int cont = 0;

    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion currentRotation, int ID)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.transform.GetChild(0).rotation = currentRotation;
        placedGameObjects.Add(newObject);
        SerializableObject serializableObject = new()
        {
            ID = ID,
            x = Mathf.RoundToInt(position.x),
            y = Mathf.RoundToInt(position.y),
            z = Mathf.RoundToInt(position.z),
            rotation = currentRotation.eulerAngles.y
        };
        serializableObjects.Add(serializableObject);
        if (ID == 6 && editmode)
        {
            metaButton.interactable = false;
            inputManager.Deselect();
            hayMeta = true;
        }
        if (ID == 7 && editmode)
        {
            cont += 1;
            hayCheckpoint = true;
        }
        escenarioProbado = false;

        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }

        
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
        if (serializableObjects[gameObjectIndex].ID == 6)
        {
            metaButton.interactable = true;
            hayMeta = false;
        }
        if (serializableObjects[gameObjectIndex].ID == 7)
        {
            cont -= 1;
            if (cont <= 0)
            {
                hayCheckpoint = false;
            }
        }

        serializableObjects[gameObjectIndex].ID = -1;
    }
}


[System.Serializable]
public class SerializableObject
{
    public int ID;
    public int x;
    public int y;
    public int z;
    public float rotation;

}
