using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulkGrabFollow : MonoBehaviour
{
    private Vector3 _relativePosition;
    private Transform _followedTransform;
    private Quaternion _initialFollowedRotation;
    private Quaternion _initialSelfRotation;
    private Quaternion _relativeRot;

    private void Start()
    {
        enabled = false;
    }

    public void Initialize(Transform followedTransform)
    {
        Transform t = transform;
        
        this._relativePosition = t.position - followedTransform.position;
        this._initialFollowedRotation = followedTransform.rotation;
        _initialSelfRotation = t.rotation;
        this._followedTransform = followedTransform;

        enabled = true;
    }

    private void Update()
    {
        Quaternion followedRot = _followedTransform.rotation;
        
        transform.position = _followedTransform.position + ((followedRot * Quaternion.Inverse(_initialFollowedRotation)) * _relativePosition);
        transform.rotation = (followedRot * Quaternion.Inverse(_initialFollowedRotation)) * _initialSelfRotation;
    }
}
