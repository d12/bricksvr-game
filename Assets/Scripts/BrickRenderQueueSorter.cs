using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BrickRenderQueueSorter
{
    private static int nextRenderQueue = 5; // Don't go over 2500! Or we leak into transparent queues
    private static Dictionary<Mesh, Material> _meshToMatMap = new Dictionary<Mesh, Material>();

    public static Material SortedMaterialFromMesh(Mesh mesh, Material sharedMaterial)
    {
        if (_meshToMatMap.ContainsKey(mesh)) return _meshToMatMap[mesh];

        return GenerateMaterialForMesh(mesh, sharedMaterial);
    }

    private static Material GenerateMaterialForMesh(Mesh mesh, Material sharedMaterial)
    {
        Material newMaterial = new Material(sharedMaterial);

        newMaterial.renderQueue = nextRenderQueue;
        nextRenderQueue += 1;

        _meshToMatMap[mesh] = newMaterial;
        return newMaterial;
    }
}
