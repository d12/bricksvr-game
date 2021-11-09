using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeepPlayerOutOfWalls : MonoBehaviour
{
    public XRRig rig;
    public LayerMask layerMask;
    public AdjustPlayerScale adjustPlayerScale;

    private Vector3 _lastGoodPosition;
    private Transform _transform;
    private SphereCollider _collider;
    private bool _wallClipAllowed;

    private readonly List<Collider> _collidersCollidingWith = new List<Collider>();
    private bool CollidingWithObjects => _collidersCollidingWith.Count > 0;

    private void Start()
    {
        _transform = transform;
        _collider = GetComponent<SphereCollider>();
    }

    private void LateUpdate()
    {
        if (_wallClipAllowed)
            return;

        if (CollidingWithObjects)
        {
            GetPlayerOutOfWall();
        }
        else
        {
            _lastGoodPosition = _transform.position;
        }
    }

    public void SetIsAllowedToGoThroughBricks(bool allowed)
    {
        _wallClipAllowed = allowed;
    }

    private void GetPlayerOutOfWall()
    {
        Vector3 movementToGetOutOfWall = _lastGoodPosition - _transform.position;
        for (int i = 0; i < 5; i++)
        {
            rig.transform.position += movementToGetOutOfWall;
            if (IsOutOfWall(_transform.position))
                return;
        }
    }

    public bool IsHeadPositionAllowed(Vector3 position)
    {
        return _wallClipAllowed || IsOutOfWall(position);
    }

    private bool IsOutOfWall(Vector3 position)
    {
        return !Physics.CheckSphere(position, _collider.radius * adjustPlayerScale.GetScale(), layerMask);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_collidersCollidingWith.Contains(other))
            _collidersCollidingWith.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_collidersCollidingWith.Contains(other))
            _collidersCollidingWith.Remove(other);
    }
}
