using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AdjustHandColliderOnPose : MonoBehaviour
{
    private XRDirectInteractor _interactor;

    private BoxCollider _collider;

    private bool _currentlySelecting;

    private readonly Vector3 _bigColliderSize = new Vector3(0.04806168f, 0.08217081f, 0.09331939f);
    private Vector3 _bigColliderCenter = new Vector3(-0.02530337f, -0.04031213f, 0.03217915f);
    private readonly Vector3 _smallColliderSize = new Vector3(0.08199464f, 0.06780242f, 0.09213501f);
    private Vector3 _smallColliderCenter = new Vector3(-0.02594286f, -0.02749649f, -0.04767084f);

    public bool rightHand;

    // Start is called before the first frame update
    private void Start()
    {
        _interactor = GetComponent<XRDirectInteractor>();
        _collider = GetComponent<BoxCollider>();
        if (rightHand)
        {
            _bigColliderCenter.x *= -1;
            _smallColliderCenter.x *= -1;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_currentlySelecting && _interactor.isSelectActive)
        {
            _currentlySelecting = true;

            Wait.ForFrames(2, () =>
            {
                if (_currentlySelecting)
                {
                    _collider.size = _smallColliderSize;
                    _collider.center = _smallColliderCenter;
                }
            });
        }
        else if(_currentlySelecting && !_interactor.isSelectActive)
        {
            _currentlySelecting = false;
            _collider.size = _bigColliderSize;
            _collider.center = _bigColliderCenter;
        }
    }
}
