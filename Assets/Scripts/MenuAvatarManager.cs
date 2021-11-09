using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAvatarManager : MonoBehaviour
{
    public GameObject head;

    public GameObject leftHand;

    public GameObject rightHand;

    public GameObject positionInMenuObject;

    public Color primaryColor;
    public Color secondaryColor;

    public Image eyesImage;
    public Image mouthImage;

    public SetColorOnPrefabBrick[] primaryBrickColors;
    public SetColorOnPrefabBrick[] secondaryBrickColors;

    public SetHandColors[] setHandColors;

    public GameObject playerHead;

    public float bounceSpeed;
    public float bounceHeight;
    public float handsBounceOffset;

    private float _appearanceKeyframe;

    private bool _visible;

    private const float BounceAmount = 0.2f;

    private float _defaultLeftHandY;
    private float _defaultRightHandY;
    private float _defaultHeadY;

    private float _scale = 1f;



    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = _visible ? Vector3.one : Vector3.zero;
        _appearanceKeyframe = _visible ? 1f : 0f;

        _defaultHeadY = head.transform.localPosition.y;
        _defaultLeftHandY = leftHand.transform.localPosition.y;
        _defaultRightHandY = rightHand.transform.localPosition.y;
    }

    private void Update()
    {
        if ((_visible && _appearanceKeyframe < 1f) || (!_visible && _appearanceKeyframe > 0f))
            AnimateVisibility();

        if (_visible)
            AnimateBounciness();
    }

    public void SetVisible(bool value)
    {
        _visible = value;
        transform.localScale = _visible ? Vector3.one : Vector3.zero;

        if (_visible)
        {
            transform.position = positionInMenuObject.transform.position;
            transform.LookAt(playerHead.transform.position);
            Vector3 rot = transform.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            transform.eulerAngles = rot;
        }
    }

    public void SetPrimaryColor(int color)
    {
        primaryColor = ColorInt.IntToColor32(color);

        foreach (SetColorOnPrefabBrick primaryBrickColorSetter in primaryBrickColors)
            primaryBrickColorSetter.SetColor(primaryColor);

        foreach (SetHandColors setHandColor in setHandColors)
            setHandColor.SetPrimaryColor(primaryColor);
    }

    public void SetSecondaryColor(int color)
    {
        secondaryColor = ColorInt.IntToColor32(color);

        foreach (SetColorOnPrefabBrick secondaryBrickColorSetter in secondaryBrickColors)
            secondaryBrickColorSetter.SetColor(secondaryColor);

        foreach (SetHandColors setHandColor in setHandColors)
            setHandColor.SetSecondaryColor(secondaryColor);
    }

    public void SetEyes(int value)
    {
        eyesImage.sprite = AvatarFaceGetter.GetInstance().GetEyes(value);
    }

    public void SetScale(int sliderValue)
    {
        _scale = AdjustPlayerScale.GetScaleFromSliderValue(sliderValue);
        transform.localScale = Vector3.one * _scale;
    }

    public void SetMouth(int value)
    {
        mouthImage.sprite = AvatarFaceGetter.GetInstance().GetMouth(value);
    }

    private void AnimateVisibility()
    {
        _appearanceKeyframe = _visible ? Mathf.Clamp01(_appearanceKeyframe + 0.04f) : Mathf.Clamp01(_appearanceKeyframe - 0.04f);
        transform.localScale = Vector3.one  * (_appearanceKeyframe * _scale);
    }

    private void AnimateBounciness()
    {
        AnimateHead();
        AnimateHands();
    }

    private void AnimateHead()
    {
        Vector3 headPos = head.transform.localPosition;
        headPos.y = _defaultHeadY + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        head.transform.localPosition = headPos;
    }

    private void AnimateHands()
    {
        Vector3 leftHandPos = leftHand.transform.localPosition;
        Vector3 rightHandPos = rightHand.transform.localPosition;

        leftHandPos.y = _defaultLeftHandY + Mathf.Sin((Time.time * bounceSpeed) + handsBounceOffset) * bounceHeight;
        rightHandPos.y = _defaultRightHandY + Mathf.Sin((Time.time * bounceSpeed) + handsBounceOffset) * bounceHeight;

        leftHand.transform.localPosition = leftHandPos;
        rightHand.transform.localPosition = rightHandPos;
    }
}

// 3.5, 0.02, 0.56