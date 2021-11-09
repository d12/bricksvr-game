using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.Networking;
using File = System.IO.File;

public class AnalyticsController : MonoBehaviour
{
    public NormalSessionManager normalSessionManager;

    private float _nextUpdate = 0f;

    // Update is called once per frame
    private void Update()
    {
        if (Time.time < _nextUpdate)
            return;

        if (!normalSessionManager.Connected())
            return;

        _nextUpdate = Time.time + 10f;

        string roomName = normalSessionManager.GetRoomName();
        string uuid = UserId.Get();
        string token = AnalyticsToken(uuid, roomName);
        string url = AnalyticsUrl(uuid, roomName, token, ReleaseVersion.MinorString());

        UnityWebRequest.Get(url).SendWebRequest();
    }

    private string AnalyticsUrl(string userId, string roomName, string token, string minorVersion)
    {
        return "";
    }
}