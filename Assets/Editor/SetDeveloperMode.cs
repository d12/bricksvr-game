using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SetDeveloperMode
{
    [MenuItem("Debug/Enable Unity Developer Mode")]
    public static void EnableDeveloperMode()
    {
        UnityEditor.EditorPrefs.SetBool("DeveloperMode", true);
    }

    [MenuItem("Debug/Disable Unity Developer Mode")]
    public static void DisableDeveloperMode()
    {
        UnityEditor.EditorPrefs.SetBool("DeveloperMode", false);
    }
}
