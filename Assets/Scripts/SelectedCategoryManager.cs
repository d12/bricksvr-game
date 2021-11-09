using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedCategoryManager : MonoBehaviour
{
    public AnimateBrickCategoryImages selectedBrickCategory;
    public GameObject tabButtonContainer;
    private Transform _transform;
    private (AnimateBrickCategoryImages, Transform)[] _brickCategories;
    private Vector3 _lastPos;

    public Transform sliderBeginObject;
    public Transform sliderEndObject;

    private Vector3 SliderBegin => sliderBeginObject.position;
    private Vector3 SliderEnd => sliderEndObject.position;

    private Vector3 _lastLocalSliderBegin;

    private bool _isGrabbed;
    private float _timeReleased;
    public float timeForSliderToSnap = 0.25f;

    public BrickPickerManager brickPickerManager;
    private HapticsManager _hapticsManager;

    private void Awake()
    {
        _transform = transform;
        _lastPos = _transform.position;
        _hapticsManager = HapticsManager.GetInstance();

        AnimateBrickCategoryImages[] animateCategoryImages =
            tabButtonContainer.GetComponentsInChildren<AnimateBrickCategoryImages>();

        _brickCategories = new (AnimateBrickCategoryImages, Transform)[animateCategoryImages.Length];

        for (int i = 0; i < animateCategoryImages.Length; i++)
            _brickCategories[i] = (animateCategoryImages[i], animateCategoryImages[i].transform);
    }

    // Start is called before the first frame update
    private void Start()
    {
        selectedBrickCategory = _brickCategories.First().Item1;
        selectedBrickCategory.active = true;

        _transform.position = SliderBegin;
    }

    private void Update()
    {
        if (!_isGrabbed && (_timeReleased != 0f))
        {
            GravitateSliderTowardsNearestItem();
        }
    }

    public void SetPosition(Vector3 pos, bool leftHand)
    {
        if (sliderBeginObject.localPosition != _lastLocalSliderBegin)
        {
            _transform.position = SliderBegin;
            UpdateSliderPosition(pos);
            _lastLocalSliderBegin = SliderBegin;
        }

        if (pos == _lastPos)
            return;

        UpdateSliderPosition(pos);

        AnimateBrickCategoryImages closestBrickCategory = ClosestBrickCategoryImage();
        if (closestBrickCategory == selectedBrickCategory) return;

        selectedBrickCategory.active = false;
        closestBrickCategory.active = true;

        selectedBrickCategory = closestBrickCategory;
        brickPickerManager.TabClicked(selectedBrickCategory.categoryName);
        _hapticsManager.PlayHaptics(0.5f, 0.5f, 0.05f, !leftHand, leftHand);
    }

    private void GravitateSliderTowardsNearestItem()
    {
        Vector3 targetPosition =
            GetClosestPointOnFiniteLine(selectedBrickCategory.transform.position, SliderBegin, SliderEnd);
        _transform.position = Vector3.Lerp(_transform.position, targetPosition,
            (Time.time - _timeReleased) / timeForSliderToSnap);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(SliderBegin, 0.02f);
        Gizmos.DrawSphere(SliderEnd, 0.02f);
    }

    private void UpdateSliderPosition(Vector3 pos)
    {
        _transform.position = GetClosestPointOnFiniteLine(pos, SliderBegin, SliderEnd);

        _lastPos = _transform.position;
    }

    public void PlayerGrabbed()
    {
        _isGrabbed = true;
    }

    public void PlayerReleased()
    {
        _isGrabbed = false;
        _timeReleased = Time.time;
    }

    private AnimateBrickCategoryImages ClosestBrickCategoryImage()
    {
        int closestIndex = 0;
        float leastDistance = 1000f;

        for (int i = 0; i < _brickCategories.Length; i++)
        {
            float distance = Vector3.Distance(_lastPos, _brickCategories[i].Item2.position);
            if (!(distance < leastDistance)) continue;

            leastDistance = distance;
            closestIndex = i;
        }

        return _brickCategories[closestIndex].Item1;
    }

    private static Vector3 GetClosestPointOnInfiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return lineStart + Vector3.Project(point - lineStart, lineEnd - lineStart);
    }

    private static Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();
        float projectLength = Mathf.Clamp(Vector3.Dot(point - lineStart, lineDirection), 0f, lineLength);
        return lineStart + lineDirection * projectLength;
    }
}
