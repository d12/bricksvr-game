using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Normal.Realtime;
using UnityEngine;

public class OwnedPhysicsBricksStore : MonoBehaviour
{
    private static OwnedPhysicsBricksStore _instance;
    public static OwnedPhysicsBricksStore GetInstance()
    {
        if(_instance == null)
            _instance = GameObject.FindWithTag("OwnedPhysicsBricksStore").GetComponent<OwnedPhysicsBricksStore>();

        return _instance;
    }

    private const int MaxPhysicsBricks = 10;

    private List<(GameObject, RealtimeTransform)> _bricks = new List<(GameObject, RealtimeTransform)>();
    private Dictionary<GameObject, bool> _bricksDict = new Dictionary<GameObject, bool>();
    private BrickDestroyer _brickDestroyer;

    // Start is called before the first frame update
    void Start()
    {
        _brickDestroyer = BrickDestroyer.GetInstance();
    }

    public void AddBrick(GameObject o)
    {
        if (_bricksDict.ContainsKey(o))
            return;

        RealtimeTransform rt = o.GetComponent<RealtimeTransform>();
        if (rt == null || !rt.isOwnedLocallySelf)
            return;

        RemoveDeadBricks();

        _bricks.Add((o, rt));
        _bricksDict.Add(o, true);

        if (_bricks.Count > MaxPhysicsBricks)
        {
            DeleteLastBrick();
        }
    }

    private void RemoveDeadBricks()
    {
        _bricks.RemoveAll(tuple => tuple.Item1 == null || tuple.Item2 == null || !tuple.Item2.isOwnedLocallySelf);
        foreach (GameObject o in _bricksDict.Keys.Where(o => o == null).ToArray())
        {
            _bricksDict.Remove(o);
        }
    }

    private void DeleteLastBrick()
    {
        (GameObject o, RealtimeTransform rt) = _bricks[0];

        _bricks.RemoveAt(0);
        _bricksDict.Remove(o);

        if(o != null)
            _brickDestroyer.DelayedDestroy(o);
    }
}
