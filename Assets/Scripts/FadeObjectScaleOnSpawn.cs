using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectScaleOnSpawn : MonoBehaviour
{
    private Vector3 _initialScale;
    private static readonly float _fadeDuration = 0.15f;
    private Transform _t;
    private float _timeStartedFading;
    public GameObject objectToScale;

    private static readonly Vector3 ScaleToStartAt = new Vector3(0.25f, 0.25f, 0.25f);

    private bool _fadeBackwards = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (objectToScale == null) objectToScale = gameObject;
        _t = objectToScale.transform;
        _initialScale = _t.localScale;
        _t.localScale = _fadeBackwards ? (Vector3.one - ScaleToStartAt) : ScaleToStartAt;
        _timeStartedFading = Time.time;
    }

    public void SetBackwardsFade()
    {
        _fadeBackwards = true;
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
                objectToScale.SetActive(false);
                _t.localScale = _initialScale;
            }
            else
            {
                _t.localScale = _initialScale * 1.0f;
            }
            enabled = false;
            _fadeBackwards = false;
        }
        else
        {
            _t.localScale = _initialScale * fadePercent;
        }
    }
}
