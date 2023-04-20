using System.Collections.Generic;
using System.Linq;
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

    private List<GameObject> _bricks = new List<GameObject>();
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

        // Make a better way of detecting placed bricks.
        BrickAttach attach = o.GetComponent<BrickAttach>();
        if(!attach.swapPrefab.Contains("Placed")) return;

        RemoveDeadBricks();

        _bricks.Add(o);
        _bricksDict.Add(o, true);

        if (_bricks.Count > MaxPhysicsBricks)
        {
            DeleteLastBrick();
        }
    }

    private void RemoveDeadBricks()
    {
        _bricks.RemoveAll(brick => brick == null);
        foreach (GameObject o in _bricksDict.Keys.Where(o => o == null).ToArray())
        {
            _bricksDict.Remove(o);
        }
    }

    private void DeleteLastBrick()
    {
        GameObject brick = _bricks[0];

        _bricks.RemoveAt(0);
        _bricksDict.Remove(brick);

        if(brick != null)
            _brickDestroyer.DelayedDestroy(brick);
    }
}
