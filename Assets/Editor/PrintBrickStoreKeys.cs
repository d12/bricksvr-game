using UnityEngine;
using UnityEditor;

public static class PrintBrickStoreKeys
{
    [MenuItem("Debug/Print Brick Store Keys")]
    public static void PrintBrickKeys()
    {
        foreach (string s in BrickStore.GetInstance().Keys())
        {
            Debug.Log(s);
        }
    }
}