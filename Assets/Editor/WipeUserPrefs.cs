using UnityEngine;
using UnityEditor;

public static class WipeUserPrefs
{
    [MenuItem("Debug/Wipe user preferences")]
    public static void WipePrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Done!");
    }
}