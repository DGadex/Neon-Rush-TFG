using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> placedGameObjects = new();

    
    [SerializeField]
    public List<SerializableObject> serializableObjects = new();

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
