using UnityEngine;
using UnityEditor;

public static class ClearUserName
{
    [MenuItem("Debug/Clear user nickname")]
    public static void ClearUserNickname()
    {
        PlayerPrefs.SetString("nickname", "");
        Debug.Log("Done!");
    }
}