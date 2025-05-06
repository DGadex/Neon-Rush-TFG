using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion currentRotation)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.transform.GetChild(0).rotation = currentRotation;
        placedGameObjects.Add(newObject);
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
