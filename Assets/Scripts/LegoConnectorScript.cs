using System.Collections.Generic;
using UnityEngine;
using System;

public class LegoConnectorScript : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> connectorsCollidingWith;

    public bool female;
    private string _otherLabel;

    private Collider[] _colliderBuffer;
    private BoxCollider _collider;

    private GameObject _brick;
    private BrickAttach _brickAttach;

    public bool covered;
    public bool outOfRenderDistance;

    public bool placed;

    private string _uuid;

    private void Awake()
    {
        if (female)
        {
            _otherLabel = "Connector-Male";
        }
        else
        {
            _otherLabel = "Connector-Female";
        }

        _colliderBuffer = new Collider[4];
        _collider = GetComponent<BoxCollider>();

        _brick = transform.parent.parent.gameObject;
        _brickAttach = _brick.GetComponent<BrickAttach>();

        if (_brickAttach != null && _collider != null)
            Physics.IgnoreCollision(_brick.GetComponent<Collider>(), _collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_otherLabel))
        {
            if (!connectorsCollidingWith.Contains(other.gameObject))
            {
                connectorsCollidingWith.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (connectorsCollidingWith.Contains(other.gameObject))
        {
            connectorsCollidingWith.Remove(other.gameObject);
        }
    }

    public GameObject ClosestConnector()
    {
        connectorsCollidingWith.RemoveAll(c => c == null);

        GameObject closestConnector = null;
        float closestDistance = 100f;
        foreach (GameObject connector in connectorsCollidingWith)
        {
            float distance = Vector3.Distance(GetConnectorPosFromCollider(gameObject), GetConnectorPosFromCollider(connector));
            if (distance < closestDistance)
            {
                closestConnector = connector;
                closestDistance = distance;
            }
        }

        return closestConnector;
    }

    private static Vector3 GetConnectorPosFromCollider(GameObject connector)
    {
        return connector.transform.TransformPoint(connector.GetComponent<BoxCollider>().center);
    }

    // Checks if this connector should be enabled
    // If not, we turn off the colliders and renderers.
    public void RecalculateEnabled()
    {
        if (BrickCollidingWith("") == null) { EnableConnector(); }
        else { DisableConnector(); }
    }

    private void EnableConnector()
    {
        covered = false;
        SetConnectorColliderEnabled();
    }

    private void DisableConnector()
    {
        if (!placed)
            return;

        covered = true;
        SetConnectorColliderEnabled();
    }

    public void SetOutOfRenderDistance(bool isOutOfRenderDistance)
    {
        this.outOfRenderDistance = isOutOfRenderDistance;
        SetConnectorColliderEnabled();
    }

    private void SetConnectorColliderEnabled()
    {
        _collider.enabled = !covered && !outOfRenderDistance;
    }

    // Pass allowedUuids to skip all bricks with UUIDs not in allowedUuids
    // TODO: This does not return base plates YET. Fixing the mask issue below should fix it.
    public GameObject BrickCollidingWith(string allowedUuids)
    {
        if (!this) return null;

        Array.Clear(_colliderBuffer, 0, _colliderBuffer.Length);

        int layerMask = allowedUuids == null
            ? LayerMask.GetMask("placed lego", "lego")
            : LayerMask.GetMask("placed lego");
        Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 4, _colliderBuffer, transform.rotation, layerMask, QueryTriggerInteraction.Ignore);

        foreach (Collider c in _colliderBuffer)
        {
            if (c == null || c.gameObject == null) continue;

            // Only bricks have BrickAttaches.
            BrickAttach attach = c.gameObject.GetComponentInParent<BrickAttach>();
            if (attach == null) continue;

            // Don't disable the collider if we're touching a brick we aren't attached to
            // if (allowedUuids != null && !allowedUuids.Contains(attach.GetUuid())) continue;

            GameObject brick = attach.gameObject;
            if (brick == _brick) continue;

            return brick;
        }

        return null;
    }

    // Not unique across m and f connectors.
    public string Uuid()
    {
        if (_uuid == null)
            _uuid = _brickAttach.GetUuid() + name;

        return _uuid;
    }
}
