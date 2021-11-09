using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TakeScreenshot
{
    [MenuItem("Helpers/Screenshot _g")]
    public static void Screenshot()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Cannot take a screenshot unless the game is playing.");
            return;
        }

        string path = $"Screenshots/{System.Guid.NewGuid()}.png";

        ScreenCapture.CaptureScreenshot(path, 4);

        Debug.Log($"Saved screenshot in {path}");
    }
}
