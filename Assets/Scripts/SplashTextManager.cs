using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using TMPro;

public class SplashTextManager : MonoBehaviour
{
    public TextMeshProUGUI splash;

    private void OnEnable() {
        UpdateSplashText();
    }

    private List<string> messages = new List<string>() {
        "Create or load a save to build!",
        $"{UnixTimeToDateTime(1681130200).ToShortDateString()} o7",
        "He who moos, lies below.",
    };

    public string GetRandomSplashText() {
        return messages[UnityEngine.Random.Range(0, messages.Count)];
    }

    public void UpdateSplashText() {
        splash.text = GetRandomSplashText();
    }

    #if UNITY_EDITOR
    [MenuItem("Debug/Update Splash Text")]
    public static void EditorSplashText()
    {
        if(!Application.isPlaying) return;

        SplashTextManager textManager = FindObjectOfType<SplashTextManager>();
        textManager.UpdateSplashText();
    }
    #endif

    public static DateTime UnixTimeToDateTime(long unix)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unix).ToLocalTime();
        return dateTime;
    }
}
