using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData
{
    [SerializeField]
    public Dictionary<Vector3Int, PlacementData> placedObjects = new();

    [SerializeField]
    private Grid grid;

    private int sizeX = 150, sizeY = 50, sizeZ = 150;


    public void AddObjectAt(Vector3Int gridPosition,
                            Vector3Int objectSize,
                            int ID,
                            int placedObjectsIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectsIndex);
        foreach(var pos in positionToOccupy)
        {
            if(placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector3Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                for(int z = 0; z < objectSize.z; z++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x,y,z));
                }
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector3Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if(placedObjects.ContainsKey(pos))
                return false;
            if(pos.x < -sizeX || pos.y < 0 || pos.z < -sizeZ)
                return false;
            if(pos.x >= sizeX || pos.y >= sizeY || pos.z >= sizeZ)
                return false;
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if(placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectsIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach(var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

[System.Serializable]
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;

    public int ID {get; private set;}
    public int PlacedObjectsIndex {get; private set;}

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectsIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectsIndex = placedObjectsIndex;
    }
}