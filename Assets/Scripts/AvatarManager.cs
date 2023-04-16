using System.Collections.Generic;
using UnityEngine;
using System;

public class AvatarManager : MonoBehaviour
{
    private static AvatarManager _instance;
    public static AvatarManager GetInstance() {
        if(_instance == null) return _instance = FindObjectOfType<AvatarManager>();
        return _instance;
    }

    public PlayerAvatar localAvatar {
        get {
            return avatars[-1];
        }
    }

    private Dictionary<int, PlayerAvatar> _avatars;
    public Dictionary<int, PlayerAvatar> avatars {
        get {
            return _avatars;
        }
    }

    public event Action<PlayerAvatar> avatarCreated;
    public event Action<PlayerAvatar> avatarDestroyed;
    
    void Start()
    {
        _avatars = new Dictionary<int, PlayerAvatar>();
    }
}
