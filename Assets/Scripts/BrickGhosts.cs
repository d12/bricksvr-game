using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public static class BrickGhosts
{
    private static GameObject _ghostPrefab;
    private static readonly Dictionary<string, Mesh> MeshCache = new Dictionary<string, Mesh>();

    public static GameObject GhostForBrick(string prefabName)
    {
        GameObject newGhost = GameObject.Instantiate(GetGhostPrefab());
        newGhost.GetComponentInChildren<MeshFilter>().mesh = GetBrickMesh(prefabName);

        return newGhost;
    }

    private static GameObject GetGhostPrefab()
    {
        if (_ghostPrefab == null) _ghostPrefab = Resources.Load<GameObject>("BrickGhost");
        return _ghostPrefab;
    }

    private static Mesh GetBrickMesh(string prefabName)
    {
        if (MeshCache.ContainsKey(prefabName)) return MeshCache[prefabName];

        MeshCache[prefabName] = Resources.Load<Mesh>($"BrickModels/{BrickData.BrickByPrefabName(prefabName).CompleteMeshPath}");
        return MeshCache[prefabName];
    }
}
