using UnityEngine;
using System.Collections.Generic;

public static class MeshCombiner
{
    public static Mesh CombineMeshes(GameObject rootObject)
    {
        MeshFilter[] meshFilters = rootObject.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            CombineInstance ci = new CombineInstance
            {
                mesh = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };
            combineInstances.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = rootObject.name + "_Combined";
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        return combinedMesh;
    }
}
