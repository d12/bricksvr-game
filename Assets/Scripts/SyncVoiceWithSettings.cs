using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class SyncVoiceWithSettings : MonoBehaviour
{
    private UserSettings _userSettings;

    private bool _self;
    private bool _pushToTalkEnabled;
    private bool _micEnabled;
    public bool Muted { get; private set; }

    public RealtimeAvatar realtimeAvatar;

    public RealtimeAvatarVoice realtimeVoice;
    // Start is called before the first frame update
    private void Start()
    {
        _userSettings = GameObject.FindWithTag("UserSettings").GetComponent<UserSettings>();
        _self = realtimeAvatar.isOwnedLocallyInHierarchy;

        // Pull current settings values
        if (_self) SetMicEnabled(_userSettings.MicrophoneEnabled);
        if (!_self) SetMicEnabled(_userSettings.OtherPlayersMicsEnabled);

        // Register for future events
        if(_self) _userSettings.MicrophoneEnabledUpdated.AddListener(SetMicEnabled);
        if (!_self) _userSettings.OtherPlayersMicrophonesEnabledUpdated.AddListener(SetMicEnabled);

        if (_self)
        {
            _userSettings.PushToTalkEnabledUpdated.AddListener(PushToTalkToggled);
            _pushToTalkEnabled = _userSettings.PushToTalkEnabled;
        }
    }

    public void ToggleMuted()
    {
        SetMuted(!Muted);
    }

    private void SetMuted(bool muted)
    {
        if (_self) return;
        Muted = muted;

        if (Muted)
            realtimeVoice.mute = true;
        else
            realtimeVoice.mute = !_micEnabled;
    }

    private void Update()
    {
        if (!_self) return;
        if (!_pushToTalkEnabled) return;
        if (!_micEnabled)
        {
            realtimeVoice.mute = true;
            return;
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch) ||
            OVRInput.Get(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch) ||
            Input.GetKey(KeyCode.T))
        {
            realtimeVoice.mute = false;
        }
        else
        {
            realtimeVoice.mute = true;
        }
    }

    private void PushToTalkToggled(bool pushToTalkEnabled)
    {
        if (!_self) return;
        _pushToTalkEnabled = pushToTalkEnabled;

        if (pushToTalkEnabled)
        {
            realtimeVoice.mute = true;
        }
        else
        {
            realtimeVoice.mute = !_userSettings.MicrophoneEnabled;
        }
    }

    private void SetMicEnabled(bool value)
    {
        _micEnabled = value;
        if (_self && _userSettings.PushToTalkEnabled) return;
        if (!_self && Muted) return;

        realtimeVoice.mute = !value;
    }
}
