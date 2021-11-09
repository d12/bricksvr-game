using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class CollapsibleColorPicker : MonoBehaviour
{
    public bool isOpen = true;
    public float animationTime;
    public float timeBetweenAnimations;

    private bool _isOpening;
    private bool _isClosing;

    private float _panelRightOpen;
    public float panelRightClosed;
    private bool IsAnimating => _isClosing || _isOpening;

    private float _animationStart;
    private float AnimationEnd => _animationStart + animationTime;

    public Transform hueSlider;
    private Vector3 _hueSliderLocalPosOpen;

    public Transform satSlider;
    private Vector3 _satSliderLocalPosOpen;

    public Transform valSlider;
    private Vector3 _valSliderLocalPosOpen;

    public Vector3 sliderClosedPos;

    public RectTransform panelBackground;
    public RectTransform panelTitle;
    public float panelTitlePosClosed;
    private float _panelTitlePosOpen;
    private Vector3 _panelTitleOriginalScale;

    private float _sliderOriginalScale;

    public Collider copyBrickColorCollider;
    public Transform collapseButton;
    private Vector3 _collapseButtonOriginalRot;

    private void Awake()
    {
        _hueSliderLocalPosOpen = hueSlider.localPosition;
        _satSliderLocalPosOpen = satSlider.localPosition;
        _valSliderLocalPosOpen = valSlider.localPosition;
        _panelRightOpen = panelBackground.GetRight();
        _sliderOriginalScale = hueSlider.localScale.x;
        _panelTitlePosOpen = panelTitle.localPosition.x;
        _panelTitleOriginalScale = panelTitle.localScale;
        _collapseButtonOriginalRot = collapseButton.localEulerAngles;
    }

    private void Update()
    {
        if (IsAnimating)
            Animation(_isClosing);
        // if (_isClosing)
        //     Animation();
        // else if (_isOpening)
        //     OpeningAnimation();
    }

    private float AnimationProgress()
    {
        return Mathf.Clamp01((Time.time - _animationStart) / (AnimationEnd - _animationStart));
    }

    public void Toggle()
    {
        if (IsAnimating) return;

        if (isOpen)
            _isClosing = true;
        else
            _isOpening = true;

        copyBrickColorCollider.enabled = false;

        _animationStart = Time.time;
    }

    private void Animation(bool closing)
    {
        float animationProgress = AnimationProgress();
        if (closing) animationProgress = 1 - animationProgress;

        if (Time.time > _animationStart + animationTime + timeBetweenAnimations)
        {
            _isOpening = false;
            _isClosing = false;
            isOpen = !closing;
            copyBrickColorCollider.enabled = isOpen;
            return;
        }

        collapseButton.localEulerAngles = new Vector3(_collapseButtonOriginalRot.x,
            _collapseButtonOriginalRot.y, Mathf.SmoothStep(-180f, 0f, animationProgress));

        panelBackground.SetRight(Mathf.SmoothStep(panelRightClosed, _panelRightOpen, animationProgress));
        panelTitle.localPosition =
            new Vector3(Mathf.SmoothStep(panelTitlePosClosed, _panelTitlePosOpen, animationProgress), panelTitle.localPosition.y, panelTitle.localPosition.z);
        panelTitle.localScale = Mathf.SmoothStep(0f, 1f, animationProgress) * _panelTitleOriginalScale;

        Vector3 hueSliderLocalPos = hueSlider.localPosition;
        hueSlider.localPosition =
            new Vector3(Mathf.SmoothStep(sliderClosedPos.x, _hueSliderLocalPosOpen.x, animationProgress),
                hueSliderLocalPos.y, hueSliderLocalPos.z);

        Vector3 satSliderLocalPos = satSlider.localPosition;
        satSlider.localPosition =
            new Vector3(Mathf.SmoothStep(sliderClosedPos.x, _satSliderLocalPosOpen.x, animationProgress),
                satSliderLocalPos.y, satSliderLocalPos.z);

        Vector3 valSliderLocalPos = valSlider.localPosition;
        valSlider.localPosition =
            new Vector3(Mathf.SmoothStep(sliderClosedPos.x, _valSliderLocalPosOpen.x, animationProgress),
                valSliderLocalPos.y, valSliderLocalPos.z);

        Vector3 sliderLocalScale = new Vector3(_sliderOriginalScale, (_sliderOriginalScale * animationProgress),
            animationProgress < 0.2f ? 0 : (_sliderOriginalScale * animationProgress));
        hueSlider.localScale = sliderLocalScale;
        satSlider.localScale = sliderLocalScale;
        valSlider.localScale = sliderLocalScale;
    }
}
