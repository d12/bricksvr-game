using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class AvatarNicknameSync : RealtimeComponent<AvatarNicknameModel>
{
    public TextMeshProUGUI nameText;
    public Transform nameTransform;
    public Transform headTransform;
    [FormerlySerializedAs("_realtimeAvatarVoice")] public RealtimeAvatarVoice realtimeAvatarVoice;
    public Transform avatarTransform;
    private Transform _currentCameraTransform;

    public GameObject face;

    public SetColorOnPrefabBrick[] primaryHeadBricks;
    public SetColorOnPrefabBrick[] secondaryHeadBricks;

    public SetHandColors[] setHandColors;

    public Image eyesImage;
    public Image mouthImage;

    private const int NumberOfVolumeMeasurementsToSmooth = 25; // arbitrary but it works.

    private float[] _lastVolumes = new float[NumberOfVolumeMeasurementsToSmooth];
    private int _nextVolumeIndex;

    private bool _isSelf;
    private static readonly Vector3 NameTextOffset = new Vector3(0, 0.3f, 0);

    public Sprite[] volumeSprites; // Should be 3 images
    public Image volumeUIImage;

    public string Nickname => model.nickname;
    public string ShortId => model.shortId;

    public int PrimaryAvatarColor => model.primaryAvatarColor;

    public int SecondaryAvatarColor => model.secondaryAvatarColor;

    public int AvatarEyes => model.avatarEyes;

    public int AvatarMouth => model.avatarMouth;

    private void Start()
    {
        // Note, sometimes Camera.current doesn't exist here
        _currentCameraTransform = Camera.current.transform;
        if (GetComponent<RealtimeAvatar>().isOwnedLocallyInHierarchy)
        {
            UserSettings userSettings = UserSettings.GetInstance();
            SetNickname(userSettings.Nickname);
            SetShortId(UserId.GetShortId());
            SetPrimaryAvatarColor(userSettings.PrimaryAvatarColor);
            SetSecondaryAvatarColor(userSettings.SecondaryAvatarColor);
            SetAvatarEyes(userSettings.AvatarEyes);
            SetAvatarMouth(userSettings.AvatarMouth);

            userSettings.NicknameUpdated.AddListener(SetNickname);
            userSettings.PrimaryAvatarColorUpdated.AddListener(SetPrimaryAvatarColor);
            userSettings.SecondaryAvatarColorUpdated.AddListener(SetSecondaryAvatarColor);
            userSettings.AvatarEyesUpdated.AddListener(SetAvatarEyes);
            userSettings.AvatarMouthUpdated.AddListener(SetAvatarMouth);

            _isSelf = true;
            nameText.enabled = false;
            face.SetActive(false);
        }
    }

    public void Update()
    {
        if (!_isSelf)
        {
            nameTransform.position = headTransform.position + Vector3.Scale(NameTextOffset, avatarTransform.localScale);
            nameTransform.LookAt(_currentCameraTransform);

            RecordVolume();

            if(_nextVolumeIndex % NumberOfVolumeMeasurementsToSmooth == 0)
                UpdateVolumeIndicator();
        }
    }

    private void NicknameSet()
    {
        nameText.text = model.nickname;
    }

    private void PrimaryAvatarColorSet()
    {
        Color primaryColor = ColorInt.IntToColor32(model.primaryAvatarColor);
        foreach (SetColorOnPrefabBrick setColorOnPrefabBrick in primaryHeadBricks)
        {
            setColorOnPrefabBrick.SetColor(primaryColor);
        }

        foreach (SetHandColors handColorSetter in setHandColors)
        {
            handColorSetter.SetPrimaryColor(primaryColor);
        }
    }

    private void SecondaryAvatarColorSet()
    {
        Color secondaryColor = ColorInt.IntToColor32(model.secondaryAvatarColor);
        foreach (SetColorOnPrefabBrick setColorOnPrefabBrick in secondaryHeadBricks)
        {
            setColorOnPrefabBrick.SetColor(secondaryColor);
        }

        foreach (SetHandColors handColorSetter in setHandColors)
        {
            handColorSetter.SetSecondaryColor(secondaryColor);
        }
    }

    private void AvatarEyesSet()
    {
        eyesImage.sprite = AvatarFaceGetter.GetInstance().GetEyes(model.avatarEyes);
    }

    private void AvatarMouthSet()
    {
        mouthImage.sprite = AvatarFaceGetter.GetInstance().GetMouth(model.avatarMouth);
    }

    protected override void OnRealtimeModelReplaced(AvatarNicknameModel previousModel, AvatarNicknameModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.nicknameDidChange -= NicknameDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.nickname = "";
                currentModel.shortId = "";
                currentModel.avatarEyes = 1;
                currentModel.avatarMouth = 1;
            }
            NicknameSet();
            PrimaryAvatarColorSet();
            SecondaryAvatarColorSet();
            AvatarEyesSet();
            AvatarMouthSet();

            currentModel.nicknameDidChange += NicknameDidChange;
            currentModel.primaryAvatarColorDidChange += PrimaryAvatarColorDidChange;
            currentModel.secondaryAvatarColorDidChange += SecondaryAvatarColorDidChange;
            currentModel.avatarEyesDidChange += AvatarEyesDidChange;
            currentModel.avatarMouthDidChange += AvatarMouthDidChange;
        }
    }

    private void NicknameDidChange(AvatarNicknameModel model, string nickname)
    {
        NicknameSet();
    }

    private void PrimaryAvatarColorDidChange(AvatarNicknameModel model, int color)
    {
        PrimaryAvatarColorSet();
    }

    private void SecondaryAvatarColorDidChange(AvatarNicknameModel model, int color)
    {
        SecondaryAvatarColorSet();
    }

    private void AvatarEyesDidChange(AvatarNicknameModel model, int value)
    {
        AvatarEyesSet();
    }

    private void AvatarMouthDidChange(AvatarNicknameModel model, int value)
    {
        AvatarMouthSet();
    }

    private void SetNickname(string nickname)
    {
        model.nickname = nickname;
    }

    private void SetShortId(string shortId)
    {
        model.shortId = shortId;
    }

    private void SetPrimaryAvatarColor(int color)
    {
        model.primaryAvatarColor = color;
    }

    private void SetSecondaryAvatarColor(int color)
    {
        model.secondaryAvatarColor = color;
    }

    private void SetAvatarEyes(int value)
    {
        model.avatarEyes = value;
    }

    private void SetAvatarMouth(int value)
    {
        model.avatarMouth = value;
    }

    private void RecordVolume()
    {
        _lastVolumes[_nextVolumeIndex] = realtimeAvatarVoice.voiceVolume;
        _nextVolumeIndex += 1;
        _nextVolumeIndex %= NumberOfVolumeMeasurementsToSmooth;
    }

    private void UpdateVolumeIndicator()
    {
        Sprite imageToUse;
        switch (realtimeAvatarVoice.voiceVolume)
        {
            case float volume when (volume < 0.15f):
                volumeUIImage.enabled = false;
                return; // No volume image when volume is too low

            case float volume when (volume < 0.3f):
                imageToUse = volumeSprites[0];
                break;

            case float volume when (volume < 0.65f):
                imageToUse = volumeSprites[1];
                break;

            default:
                imageToUse = volumeSprites[2];
                break;
        }

        volumeUIImage.enabled = true;
        volumeUIImage.sprite = imageToUse;
    }
}
