using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SetQualityLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.SetQualityLevel(Application.platform == RuntimePlatform.Android ? 0 : 3);

        XRSettings.eyeTextureResolutionScale = (Application.platform == RuntimePlatform.Android) ? 1.0f : 1.5f;

        // float multiplier =
        //     (Application.platform == RuntimePlatform.WindowsEditor ||
        //      Application.platform == RuntimePlatform.WindowsPlayer ||
        //      Application.platform == RuntimePlatform.OSXEditor ||
        //      Application.platform == RuntimePlatform.OSXPlayer)
        //         ? 1.5f
        //         : 1.0f;
        //
        // XRSettings.eyeTextureResolutionScale =
        //     ((Screen.width / 2.0f) / XRSettings.eyeTextureWidth) * multiplier;
    }
}
