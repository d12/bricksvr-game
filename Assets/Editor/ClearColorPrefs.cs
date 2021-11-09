using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ClearColorPrefs
{

    [MenuItem("Fixup/Clear Color Prefs")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteKey("primary_avatar_color");
        PlayerPrefs.DeleteKey("secondary_avatar_color");
    }
}
