using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class DisableMicWhenEditor : MonoBehaviour
{
    public RealtimeAvatarVoice voice;
    // Start is called before the first frame update
    private void Start()
    {
        if (Application.isEditor)
        {
            voice.enabled = false;
        }
    }
}
