using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickMeshRecalculator : MonoBehaviour
{
    private static BrickMeshRecalculator _instance;

    public static BrickMeshRecalculator GetInstance()
    {
        if (_instance == null) _instance = GameObject.FindObjectOfType<BrickMeshRecalculator>();

        return _instance;
    }

    private List<BrickAttach> _attachesToRecalculate;
    private Collider[] _overlapColliders;

    // Start is called before the first frame update
    void Awake()
    {
        _attachesToRecalculate = new List<BrickAttach>();
        _overlapColliders = new Collider[500];

        _instance = this;
    }

    public void AddAttach(BrickAttach attach)
    {
        _attachesToRecalculate.Add(attach);
    }

    public void RecalculateNearbyBricks(Vector3 pos, float radius = 0.3f)
    {
        int numberOfCollisions = Physics.OverlapSphereNonAlloc(pos, radius, _overlapColliders, LayerMask.GetMask("placed lego"), QueryTriggerInteraction.Ignore);
        for (int i = 0; i < numberOfCollisions; i++)
        {
            BrickAttach attach = _overlapColliders[i].GetComponentInParent<BrickAttach>();
            if(!_attachesToRecalculate.Contains(attach)) _attachesToRecalculate.Add(attach);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_attachesToRecalculate.Count > 0)
        {
            foreach (BrickAttach attach in _attachesToRecalculate)
            {
                if (attach != null && attach.gameObject != null)
                {
                    attach.RecalculateEnabledConnectors();
                    attach.RecalculateRenderedGeometry();
                }
            }
        }

        _attachesToRecalculate.Clear();
    }
}
