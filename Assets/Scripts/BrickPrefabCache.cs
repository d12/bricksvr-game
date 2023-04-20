using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPrefabCache : MonoBehaviour
{
    private static BrickPrefabCache _instance;
    private static bool _instanceSet;

    private Dictionary<string, GameObject> _brickPrefabCache;

    public static BrickPrefabCache GetInstance()
    {
        if (_instanceSet) return _instance;

        _instance = FindObjectOfType<BrickPrefabCache>();
        _instanceSet = true;

        return _instance;
    }
    private void Awake()
    {
        _instance = this;
        _instanceSet = true;

        GenerateCache();
    }

    public GameObject Get(string brickName)
    {
        return _brickPrefabCache?[brickName];
    }

    public void GenerateCache()
    {
        _brickPrefabCache = new Dictionary<string, GameObject>();
        foreach (BrickData.Brick brick in BrickData.AllBricks())
        {
            _brickPrefabCache.Add(brick.PrefabName, Resources.Load(brick.PrefabName) as GameObject);

            GameObject placedBrickPrefab = Resources.Load(brick.PrefabName + " - Placed") as GameObject;
            _brickPrefabCache.Add(brick.PrefabName + " - Placed", placedBrickPrefab);

            // WarmGeometryAndOutlines(placedBrickPrefab);
        }
    }

    private void WarmGeometryAndOutlines(GameObject placedBrickPrefab)
    {
        GameObject newBrick = Instantiate(placedBrickPrefab, new Vector3(0, -50f, 0), Quaternion.identity);
        newBrick.GetComponent<BrickUuid>().uuid = Random.Range(0, 1000000).ToString();
        BrickAttach newBrickAttach = newBrick.GetComponent<BrickAttach>();
        newBrickAttach.RecalculateRenderedGeometry();
        BrickDestroyer.GetInstance().DelayedDestroy(newBrick);
    }
}
