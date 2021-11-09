using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class LookupRealtimeAvatarManager : MonoBehaviour
{
    private static RealtimeAvatarManager _instance;
    public static RealtimeAvatarManager GetInstance()
    {
        if (_instance == null)
            _instance = GameObject.FindGameObjectWithTag("Realtime").GetComponent<RealtimeAvatarManager>();

        return _instance;
    }
}
