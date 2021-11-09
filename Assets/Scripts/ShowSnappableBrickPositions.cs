using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSnappableBrickPositions : MonoBehaviour
{
    private BrickAttachDetector _detector;

    public GameObject currentGhostPrefab;

    private string _prefabName;

    // Is a brick ghost currently visible?
    private bool _isGhostVisible;

    private void Start()
    {
        _detector = GetComponent<BrickAttachDetector>();
        _prefabName = GetComponent<BrickAttach>().normalPrefabName;
    }

    private void OnDestroy()
    {
        Reset();
    }

    public void ResetAndDisable()
    {
        Reset();
        enabled = false;
    }

    private void Reset()
    {
        if (currentGhostPrefab != null)
        {
            Destroy(currentGhostPrefab);
            currentGhostPrefab = null;
        }

        _isGhostVisible = false;
    }

    private void Update()
    {
        (bool canConnect, Vector3 pos, Quaternion rot, Vector3 _) = _detector.CheckIfCanConnect();
        if (canConnect)
        {
            if (!currentGhostPrefab)
            {
                currentGhostPrefab = BrickGhosts.GhostForBrick(_prefabName);
            }

            _isGhostVisible = true;
            currentGhostPrefab.transform.position = pos;
            currentGhostPrefab.transform.rotation = rot;
        }
        else if (_isGhostVisible)
        {
            Reset();
        }
    }
}
