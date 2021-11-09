using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class DisableLoadingScreenWhenConnected : MonoBehaviour
{
    public Realtime realtime;
    public GameObject canvas;

    private void Start() {
        realtime.GetComponent<RealtimeAvatarManager>().avatarCreated += HideLoadingScreen;

        canvas.SetActive(true);
    }

    private void HideLoadingScreen(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar) {
        if(isLocalAvatar) canvas.SetActive(false);
    }
}
