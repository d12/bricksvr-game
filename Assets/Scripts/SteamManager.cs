using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SteamManager : MonoBehaviour
{
    [FormerlySerializedAs("steamId")] public uint steamAppId;

    private bool _initialized;
    private bool _initializationSucceeded;

    // Start is called before the first frame update
    void Awake()
    {
        if (!_initialized)
            Initialize();
    }

    public bool Initialize()
    {
        if (_initialized) return _initializationSucceeded;

        if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
        {
            _initialized = true;
            _initializationSucceeded = false;
            Debug.Log("Skipping Steam client initialization because we're not on Windows.");

            return false;
        }

        try
        {
            Steamworks.SteamClient.Init(steamAppId);
        }
        catch (Exception e)
        {
            _initialized = true;
            _initializationSucceeded = false;

            Debug.LogError("Failed to initialize steam client.");
            Debug.LogError(e);

            return false;
        }

        _initialized = true;
        _initializationSucceeded = true;
        return true;
    }

    public ulong GetUserId()
    {
        return Steamworks.SteamClient.SteamId.Value;
    }

    public void RestartAppIfNecessary()
    {
        // Quit app if we weren't launched through the steam client
        if (Steamworks.SteamClient.RestartAppIfNecessary(steamAppId))
        {
            Application.Quit();
        }
    }
}
