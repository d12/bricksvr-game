using UnityEngine;
using UnityEngine.Events;

public class UserSettings : MonoBehaviour
{
    private static UserSettings _instance;

    public static UserSettings GetInstance()
    {
        if (_instance == null) _instance = GameObject.FindWithTag("UserSettings").GetComponent<UserSettings>();

        return _instance;
    }

    [SerializeField]
    public SerializableBoolEvent TeleportTriggerEnabledUpdated;
    public SerializableBoolEvent MicrophoneEnabledUpdated;
    public SerializableBoolEvent OtherPlayersMicrophonesEnabledUpdated;
    public SerializableBoolEvent PerformanceModeEnabledUpdated;
    public SerializableBoolEvent PlayMusicEnabledUpdated;
    public SerializableBoolEvent SmoothRotationEnabledUpdated;
    public SerializableBoolEvent BrickClickSoundsEnabledUpdated;
    public SerializableBoolEvent PushToTalkEnabledUpdated;

    public SerializableFloatEvent MusicVolumeUpdated;
    public SerializableFloatEvent MovementSpeedUpdated;
    public SerializableFloatEvent RotationSpeedUpdated;
    public SerializableFloatEvent VignetteStrengthUpdated;

    public SerializableIntEvent RenderDistanceUpdated;
    public SerializableIntEvent PlayerScaleUpdated;
    public SerializableIntEvent PrimaryAvatarColorUpdated;
    public SerializableIntEvent SecondaryAvatarColorUpdated;
    public SerializableIntEvent AvatarMouthUpdated;
    public SerializableIntEvent AvatarEyesUpdated;
    public SerializableIntEvent BrickShininessUpdated;
    public SerializableIntEvent AutosaveUpdated;
    public SerializableIntEvent SkyboxUpdated;

    public SerializableStringEvent NicknameUpdated;

    [System.Serializable]
    public class SerializableBoolEvent : UnityEvent<bool> { }

    [System.Serializable]
    public class SerializableFloatEvent : UnityEvent<float> { }

    [System.Serializable]
    public class SerializableStringEvent : UnityEvent<string> { }

    [System.Serializable]
    public class SerializableIntEvent : UnityEvent<int> { }

    public void Awake()
    {
        InitializeSettings();
    }

    public void Start()
    {
        SendInitialEvents();
        // SendNicknameToServerOnLoad();
    }

    private void InitializeSettings()
    {
        if (!PlayerPrefs.HasKey(TeleportTriggersKey))               PlayerPrefs.SetInt(TeleportTriggersKey, 1);
        if (!PlayerPrefs.HasKey(MicrophoneEnabledKey))              PlayerPrefs.SetInt(MicrophoneEnabledKey, 1);
        if (!PlayerPrefs.HasKey(OtherPlayersMicrophonesEnabledKey)) PlayerPrefs.SetInt(OtherPlayersMicrophonesEnabledKey, 1);
        if (!PlayerPrefs.HasKey(PerformanceModeEnabledKey))         PlayerPrefs.SetInt(PerformanceModeEnabledKey, 0);
        if (!PlayerPrefs.HasKey(PlayMusicEnabledKey))               PlayerPrefs.SetInt(PlayMusicEnabledKey, 1);
        if (!PlayerPrefs.HasKey(SmoothRotationEnabledKey))          PlayerPrefs.SetInt(SmoothRotationEnabledKey, 0);
        if (!PlayerPrefs.HasKey(MusicVolumeKey))                    PlayerPrefs.SetFloat(MusicVolumeKey, 0.05f); // max of 0.1
        if (!PlayerPrefs.HasKey(MovementSpeedKey))                  PlayerPrefs.SetFloat(MovementSpeedKey, 0.5f);
        if (!PlayerPrefs.HasKey(RotationSpeedKey))                  PlayerPrefs.SetFloat(RotationSpeedKey, 0.5f);
        if (!PlayerPrefs.HasKey(NicknameKey))                       PlayerPrefs.SetString(NicknameKey, "");
        if (!PlayerPrefs.HasKey(BrickClickSoundsKey))               PlayerPrefs.SetInt(BrickClickSoundsKey, 1);
        if (!PlayerPrefs.HasKey(TutorialPlayedKey))                 PlayerPrefs.SetInt(TutorialPlayedKey, 0);
        if (!PlayerPrefs.HasKey(RecentRoomsKey))                    PlayerPrefs.SetString(RecentRoomsKey, "");
        if (!PlayerPrefs.HasKey(VignetteStrengthKey))               PlayerPrefs.SetFloat(VignetteStrengthKey, 0.0f);
        if (!PlayerPrefs.HasKey(RenderDistanceKey))                 PlayerPrefs.SetInt(RenderDistanceKey, 10);
        if (!PlayerPrefs.HasKey(PlayerScaleKey))                    PlayerPrefs.SetInt(PlayerScaleKey, 5); // 100%
        if (!PlayerPrefs.HasKey(PushToTalkKey))                     PlayerPrefs.SetInt(PushToTalkKey, 0); // 100%
        if (!PlayerPrefs.HasKey(PrimaryAvatarColorKey))             PlayerPrefs.SetInt(PrimaryAvatarColorKey, ColorInt.Color32ToInt(new Color32(28, 154, 251, 255)));
        if (!PlayerPrefs.HasKey(SecondaryAvatarColorKey))           PlayerPrefs.SetInt(SecondaryAvatarColorKey, ColorInt.Color32ToInt(new Color32(255, 225, 0, 255)));
        if (!PlayerPrefs.HasKey(AvatarMouthKey))                    PlayerPrefs.SetInt(AvatarMouthKey, 1);
        if (!PlayerPrefs.HasKey(AvatarEyesKey))                     PlayerPrefs.SetInt(AvatarEyesKey, 1);
        if (!PlayerPrefs.HasKey(BrickShininessKey))                 PlayerPrefs.SetInt(BrickShininessKey, 5);
        if (!PlayerPrefs.HasKey(AutosaveKey))                       PlayerPrefs.SetInt(AutosaveKey, 5);
        if (!PlayerPrefs.HasKey(SkyboxKey))                         PlayerPrefs.SetInt(SkyboxKey, 0);
    }

    private void SendInitialEvents()
    {
        TeleportTriggerEnabledUpdated?.Invoke(PlayerPrefs.GetInt(TeleportTriggersKey) == 1);
        MicrophoneEnabledUpdated?.Invoke(PlayerPrefs.GetInt(MicrophoneEnabledKey) == 1);
        OtherPlayersMicrophonesEnabledUpdated?.Invoke(PlayerPrefs.GetInt(OtherPlayersMicrophonesEnabledKey) == 1);
        PerformanceModeEnabledUpdated?.Invoke(PlayerPrefs.GetInt(PerformanceModeEnabledKey) == 1);
        PlayMusicEnabledUpdated?.Invoke(PlayerPrefs.GetInt(PlayMusicEnabledKey) == 1);
        SmoothRotationEnabledUpdated?.Invoke(PlayerPrefs.GetInt(SmoothRotationEnabledKey) == 1);
        // MusicVolumeUpdated?.Invoke(PlayerPrefs.GetFloat(MusicVolumeKey));
        // Skip this one
        MovementSpeedUpdated?.Invoke(PlayerPrefs.GetFloat(MovementSpeedKey));
        RotationSpeedUpdated?.Invoke(PlayerPrefs.GetFloat(RotationSpeedKey));
        NicknameUpdated?.Invoke(PlayerPrefs.GetString(NicknameKey));
        BrickClickSoundsEnabledUpdated?.Invoke(PlayerPrefs.GetInt(BrickClickSoundsKey) == 1);
        // RecentRooms has no event right now. Add one if it's ever needed.
        VignetteStrengthUpdated?.Invoke(PlayerPrefs.GetFloat(VignetteStrengthKey));
        RenderDistanceUpdated?.Invoke(PlayerPrefs.GetInt(RenderDistanceKey));
        PlayerScaleUpdated?.Invoke(PlayerPrefs.GetInt(PlayerScaleKey));
        PushToTalkEnabledUpdated?.Invoke(PlayerPrefs.GetInt(PushToTalkKey) == 1);
        PrimaryAvatarColorUpdated?.Invoke(PlayerPrefs.GetInt(PrimaryAvatarColorKey));
        SecondaryAvatarColorUpdated?.Invoke(PlayerPrefs.GetInt(SecondaryAvatarColorKey));
        AvatarMouthUpdated?.Invoke(PlayerPrefs.GetInt(AvatarMouthKey));
        AvatarEyesUpdated?.Invoke(PlayerPrefs.GetInt(AvatarEyesKey));
        BrickShininessUpdated?.Invoke(PlayerPrefs.GetInt(BrickShininessKey));
        AutosaveUpdated?.Invoke(PlayerPrefs.GetInt(AutosaveKey));
        SkyboxUpdated?.Invoke(PlayerPrefs.GetInt(SkyboxKey));
    }

    private void SendNicknameToServerOnLoad()
    {
        if(!string.IsNullOrEmpty(Nickname))
        {
            BrickServerInterface.GetInstance().SetNickname(Nickname);
        }
    }

    private const string TeleportTriggersKey = "teleport_triggers";
    public bool TeleportTriggersEnabled
    {
        get => PlayerPrefs.GetInt(TeleportTriggersKey) == 1;
        set
        {
            PlayerPrefs.SetInt(TeleportTriggersKey, value ? 1 : 0);
            TeleportTriggerEnabledUpdated?.Invoke(value);
        }
    }

    private const string MicrophoneEnabledKey = "microphone_enabled";
    public bool MicrophoneEnabled
    {
        get => PlayerPrefs.GetInt(MicrophoneEnabledKey) == 1;
        set
        {
            PlayerPrefs.SetInt(MicrophoneEnabledKey, value ? 1 : 0);
            MicrophoneEnabledUpdated?.Invoke(value);
        }
    }

    private const string OtherPlayersMicrophonesEnabledKey = "other_player_mics_enabled";
    public bool OtherPlayersMicsEnabled
    {
        get => PlayerPrefs.GetInt(OtherPlayersMicrophonesEnabledKey) == 1;
        set
        {
            PlayerPrefs.SetInt(OtherPlayersMicrophonesEnabledKey, value ? 1 : 0);
            OtherPlayersMicrophonesEnabledUpdated?.Invoke(value);
        }
    }

    private const string PerformanceModeEnabledKey = "super_ultra_performance_mode";
    public bool SuperUltraPerformanceMode
    {
        get => PlayerPrefs.GetInt(PerformanceModeEnabledKey) == 1;
        set
        {
            PlayerPrefs.SetInt(PerformanceModeEnabledKey, value ? 1 : 0);
            PerformanceModeEnabledUpdated?.Invoke(value);
        }
    }

    private const string PlayMusicEnabledKey = "play_music";
    public bool PlayMusicEnabled
    {
        get => PlayerPrefs.GetInt(PlayMusicEnabledKey) == 1;
        set
        {
            PlayerPrefs.SetInt(PlayMusicEnabledKey, value ? 1 : 0);
            PlayMusicEnabledUpdated?.Invoke(value);
        }
    }

    private const string SmoothRotationEnabledKey = "smooth_rotation";
    public bool SmoothRotationEnabled
    {
        get => PlayerPrefs.GetInt(SmoothRotationEnabledKey) == 1;
        set
        {
            PlayerPrefs.SetInt(SmoothRotationEnabledKey, value ? 1 : 0);
            SmoothRotationEnabledUpdated?.Invoke(value);
        }
    }

    private const string MusicVolumeKey = "music_volume";
    public float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicVolumeKey);
        set
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp(value, 0f, 0.1f));
            MusicVolumeUpdated?.Invoke(value);
        }
    }

    private const string MovementSpeedKey = "movement_speed";
    public float MovementSpeed
    {
        get => PlayerPrefs.GetFloat(MovementSpeedKey);
        set
        {
            PlayerPrefs.SetFloat(MovementSpeedKey, Mathf.Clamp(value, 0f, 1f));
            MovementSpeedUpdated?.Invoke(value);
        }
    }

    private const string RotationSpeedKey = "rotation_speed";
    public float RotationSpeed
    {
        get => PlayerPrefs.GetFloat(RotationSpeedKey);
        set
        {
            PlayerPrefs.SetFloat(RotationSpeedKey, Mathf.Clamp(value, 0f, 1f));
            RotationSpeedUpdated?.Invoke(value);
        }
    }

    private const string VignetteStrengthKey = "vignette_strength";
    public float VignetteStrength
    {
        get => PlayerPrefs.GetFloat(VignetteStrengthKey);
        set
        {
            PlayerPrefs.SetFloat(VignetteStrengthKey, Mathf.Clamp(value, 0f, 1f));
            VignetteStrengthUpdated?.Invoke(value);
        }
    }

    private const string NicknameKey = "nickname";
    public string Nickname
    {
        get => PlayerPrefs.GetString(NicknameKey);
        set
        {
            PlayerPrefs.SetString(NicknameKey, value);
            BrickServerInterface.GetInstance().SetNickname(value);
            NicknameUpdated?.Invoke(value);
        }
    }

    private const string BrickClickSoundsKey = "brick_click_sounds";
    public bool BrickClickSoundsEnabled
    {
        get => PlayerPrefs.GetInt(BrickClickSoundsKey) == 1;
        set
        {
            PlayerPrefs.SetInt(BrickClickSoundsKey, value ? 1 : 0);
            BrickClickSoundsEnabledUpdated?.Invoke(value);
        }
    }

    private const string TutorialPlayedKey = "tutorial_played";
    public bool TutorialPlayed
    {
        get => PlayerPrefs.GetInt(TutorialPlayedKey) == 1;
        set => PlayerPrefs.SetInt(TutorialPlayedKey, value ? 1 : 0);
    }

    private const string RecentRoomsKey = "recent_rooms";
    public string RecentRooms => PlayerPrefs.GetString(RecentRoomsKey);

    public void AddRecentRoom(string room)
    {
        string currentRecentRooms = RecentRooms;
        currentRecentRooms = currentRecentRooms.Replace($"{room},", "");
        currentRecentRooms = currentRecentRooms.Replace($",{room}", "");
        currentRecentRooms = currentRecentRooms.Replace($"{room}", "");

        PlayerPrefs.SetString(RecentRoomsKey, currentRecentRooms.Length == 0 ? room : $"{room},{currentRecentRooms}");
    }

    private const string RenderDistanceKey = "render_distance";
    public int RenderDistance
    {
        get => PlayerPrefs.GetInt(RenderDistanceKey);
        set
        {
            PlayerPrefs.SetInt(RenderDistanceKey, value);
            RenderDistanceUpdated?.Invoke(value);
        }
    }

    // Slider outputs a float, even when in whole numbers mode
    public float RenderDistanceF
    {
        set => RenderDistance = (int) value;
    }

    private const string PlayerScaleKey = "player_scale";
    public int PlayerScale
    {
        get => PlayerPrefs.GetInt(PlayerScaleKey);
        set
        {
            PlayerPrefs.SetInt(PlayerScaleKey, value);
            PlayerScaleUpdated?.Invoke(value);
        }
    }

    // Slider outputs a float, even when in whole numbers mode
    public float PlayerScaleF
    {
        set => PlayerScale = (int) value;
    }

    private const string PushToTalkKey = "push_to_talk";
    public bool PushToTalkEnabled
    {
        get => PlayerPrefs.GetInt(PushToTalkKey) == 1;
        set
        {
            PlayerPrefs.SetInt(PushToTalkKey, value ? 1 : 0);
            PushToTalkEnabledUpdated.Invoke(value);
        }
    }

    private const string PrimaryAvatarColorKey = "primary_avatar_color";
    public int PrimaryAvatarColor
    {
        get => PlayerPrefs.GetInt(PrimaryAvatarColorKey);
        set
        {
            PlayerPrefs.SetInt(PrimaryAvatarColorKey, value);
            PrimaryAvatarColorUpdated?.Invoke(value);
        }
    }

    private const string SecondaryAvatarColorKey = "seconary_avatar_color";
    public int SecondaryAvatarColor
    {
        get => PlayerPrefs.GetInt(SecondaryAvatarColorKey);
        set
        {
            PlayerPrefs.SetInt(SecondaryAvatarColorKey, value);
            SecondaryAvatarColorUpdated?.Invoke(value);
        }
    }

    private const string AvatarMouthKey = "avatar_mouth";
    public int AvatarMouth
    {
        get => PlayerPrefs.GetInt(AvatarMouthKey);
        set
        {
            PlayerPrefs.SetInt(AvatarMouthKey, value);
            AvatarMouthUpdated?.Invoke(value);
        }
    }

    private const string AvatarEyesKey = "avatar_eyes";
    public int AvatarEyes
    {
        get => PlayerPrefs.GetInt(AvatarEyesKey);
        set
        {
            PlayerPrefs.SetInt(AvatarEyesKey, value);
            AvatarEyesUpdated?.Invoke(value);
        }
    }

    private const string BrickShininessKey = "brick_shininess";
    public int BrickShininess
    {
        get => PlayerPrefs.GetInt(BrickShininessKey);
        set
        {
            PlayerPrefs.SetInt(BrickShininessKey, value);
            BrickShininessUpdated?.Invoke(value);
        }
    }

    public float BrickShininessF
    {
        set => BrickShininess = (int) value;
    }

    private const string AutosaveKey = "auto_save";
    public int AutoSave
    {
        get => PlayerPrefs.GetInt(AutosaveKey);
        set
        {
            PlayerPrefs.SetInt(AutosaveKey, value);
            AutosaveUpdated?.Invoke(value);
        }
    }

    public float AutoSaveF
    {
        set => AutoSave = (int) value;
    }

    private const string SkyboxKey = "skybox";
    public int Skybox
    {
        get => PlayerPrefs.GetInt(SkyboxKey);
        set
        {
            PlayerPrefs.SetInt(SkyboxKey, value);
            SkyboxUpdated?.Invoke(value);
        }
    }
}
