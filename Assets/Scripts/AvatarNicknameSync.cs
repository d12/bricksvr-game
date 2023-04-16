using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AvatarNicknameSync : AvatarNicknameModel
{
    public TextMeshProUGUI nameText;
    public Transform nameTransform;
    public Transform headTransform;
    //[FormerlySerializedAs("_realtimeAvatarVoice")] public RealtimeAvatarVoice realtimeAvatarVoice;
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

    public string Nickname => nickname;
    public string ShortId => shortId;

    public int PrimaryAvatarColor => primaryAvatarColor;

    public int SecondaryAvatarColor => secondaryAvatarColor;

    public int AvatarEyes => avatarEyes;

    public int AvatarMouth => avatarMouth;

    private void Start()
    {
        // Note, sometimes Camera.current doesn't exist here
        _currentCameraTransform = Camera.current.transform;
        if (GetComponent<PlayerAvatar>() == null)
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
        }
    }

    private void NicknameSet()
    {
        nameText.text = nickname;
    }

    private void PrimaryAvatarColorSet()
    {
        Color primaryColor = ColorInt.IntToColor32(primaryAvatarColor);
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
        Color secondaryColor = ColorInt.IntToColor32(secondaryAvatarColor);
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
        eyesImage.sprite = AvatarFaceGetter.GetInstance().GetEyes(avatarEyes);
    }

    private void AvatarMouthSet()
    {
        mouthImage.sprite = AvatarFaceGetter.GetInstance().GetMouth(avatarMouth);
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

    private void SetNickname(string _nickname)
    {
        nickname = _nickname;
    }

    private void SetShortId(string _shortId)
    {
        shortId = _shortId;
    }

    private void SetPrimaryAvatarColor(int color)
    {
        primaryAvatarColor = color;
    }

    private void SetSecondaryAvatarColor(int color)
    {
        secondaryAvatarColor = color;
    }

    private void SetAvatarEyes(int value)
    {
        avatarEyes = value;
    }

    private void SetAvatarMouth(int value)
    {
        avatarMouth = value;
    }
}
