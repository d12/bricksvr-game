using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public GameObject ownerCrown;
    public Image micImage;
    public Button micButton;

    public Sprite micMutedSprite;
    public Sprite micEnabledSprite;

    public void Initialize(PlayerAvatar avatar)
    {
        AvatarNicknameSync nicknameSync = avatar.GetComponent<AvatarNicknameSync>();
        nameText.text = nicknameSync.Nickname;

        // TODO: Multiplayer support.
        ownerCrown.SetActive(true);
        SetMicrophoneSprite();

        if (avatar.isLocal)
            micButton.interactable = false;

    }

    public void MuteButtonPressed()
    {
        //_syncVoiceWithSettings.ToggleMuted();
        SetMicrophoneSprite();
    }

    private void SetMicrophoneSprite()
    {
        //micImage.sprite = _syncVoiceWithSettings.Muted ? micMutedSprite : micEnabledSprite;
    }
}
