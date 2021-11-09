using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickMeshCache : MonoBehaviour
{
    private static BrickMeshCache _instance;
    private readonly Dictionary<string, Mesh> _meshCache = new Dictionary<string, Mesh>();

    private bool _perfMode;

    private void Awake()
    {
        _instance = this;
        _perfMode = UserSettings.GetInstance().SuperUltraPerformanceMode;
    }

    public Mesh Get(string key)
    {
        if (!_meshCache.ContainsKey(key)) return null;
        return _meshCache[key];
    }

    public void Put(string key, Mesh mesh)
    {
        _meshCache[key] = mesh;
    }

    public void Clear()
    {
        _meshCache.Clear();
    }

    // Class interface
    public static BrickMeshCache GetInstance()
    {
        if (_instance == null) _instance = FindObjectOfType<BrickMeshCache>();
        return _instance;
    }

    public void ClearCacheAndRecalculateMeshes(bool perfModeEnabled)
    {
        if (_perfMode == perfModeEnabled) return;
        _perfMode = perfModeEnabled;

        Clear();
        foreach (GameObject brick in BrickStore.GetInstance().Values())
        {
            if (brick == null) return;

            BrickAttach attach = brick.GetComponent<BrickAttach>();
            if (attach == null) return;

            attach.RecalculateRenderedGeometry();
        }
    }
}
