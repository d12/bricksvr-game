using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBrickMenu : MonoBehaviour
{
    private Vector3 _initialScale;
    private static readonly float _fadeDuration = 0.15f;
    private Transform _t;
    private float _timeStartedFading;
    public GameObject objectToScale;

    public SliderGrabbable hueGrabber;
    public SliderGrabbable satGrabber;
    public SliderGrabbable valueGrabber;

    private static readonly Vector3 ScaleToStartAt = new Vector3(0.25f, 0.25f, 0.25f);

    private bool _fadeBackwards = false;

    private bool _initialized;

    private void Initialize()
    {
        _initialized = true;
        if (objectToScale == null) objectToScale = gameObject;
        _t = objectToScale.transform;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if(!_initialized) Initialize();
        _t.localScale = _fadeBackwards ? (Vector3.one - ScaleToStartAt) : ScaleToStartAt;
        _timeStartedFading = Time.time;
    }

    public void SetBackwardsFade()
    {
        if(!_initialized) Initialize();
        _fadeBackwards = true;
    }

    public void BeginGrow()
    {
        if(!_initialized) Initialize();
        _t.localScale = ScaleToStartAt;
        enabled = true;
    }

    public void BeginShrink()
    {

        _fadeBackwards = true;
        enabled = true;
    }

    // Update is called once per frame
    private void Update()
    {
        float elapsedTime = Time.time - _timeStartedFading;
        float fadeProgress = elapsedTime / _fadeDuration;
        float fadePercent = _fadeBackwards ? (1 - fadeProgress) : fadeProgress;
        if (fadeProgress > 1f)
        {
            if (_fadeBackwards)
            {
                _t.localScale = Vector3.zero;
            }
            else
            {
                _t.localScale = Vector3.one;
            }
            enabled = false;
            _fadeBackwards = false;
        }
        else
        {
            _t.localScale = Vector3.one * fadePercent;
        }

        FixSliderPositions();
   }

    private void FixSliderPositions()
    {
        hueGrabber.FixSliderKnobPosition();
        satGrabber.FixSliderKnobPosition();
        valueGrabber.FixSliderKnobPosition();
    }
}