using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public GameObject ownerCrown;
    public Image micImage;
    public Button micButton;

    public Sprite micMutedSprite;
    public Sprite micEnabledSprite;

    private SyncVoiceWithSettings _syncVoiceWithSettings;

    public void Initialize(RealtimeAvatar avatar, RoomOwnershipSync ownershipSync)
    {
        AvatarNicknameSync nicknameSync = avatar.GetComponent<AvatarNicknameSync>();
        _syncVoiceWithSettings = avatar.head.GetComponent<SyncVoiceWithSettings>();
        nameText.text = nicknameSync.Nickname;

        ownerCrown.SetActive(ownershipSync.IsRoomOwner(nicknameSync.ShortId));
        SetMicrophoneSprite();

        if (avatar.isOwnedLocallyInHierarchy)
            micButton.interactable = false;

    }

    public void MuteButtonPressed()
    {
        _syncVoiceWithSettings.ToggleMuted();
        SetMicrophoneSprite();
    }

    private void SetMicrophoneSprite()
    {
        micImage.sprite = _syncVoiceWithSettings.Muted ? micMutedSprite : micEnabledSprite;
    }
}
