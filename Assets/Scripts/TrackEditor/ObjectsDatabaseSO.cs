using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectsData> objectsData;
}

[Serializable]
public class ObjectsData
{
    [field: SerializeField]
    public string Name {get; private set;}
    [field: SerializeField]
    public int ID {get; private set;}
    [field: SerializeField]
    public Vector3Int Size { get; private set;} = Vector3Int.one;
    [field: SerializeField]
    public GameObject Prefab {get; private set;}

}