using UnityEngine;
using UnityEditor;

public static class DebugMenu
{
    [MenuItem("Debug/Print Global Position")]
    public static void PrintGlobalPosition()
    {
        if (Selection.activeGameObject != null)
        {
            Debug.Log(Selection.activeGameObject.name + " x is at " + Selection.activeGameObject.transform.position.x);
            Debug.Log(Selection.activeGameObject.name + " y is at " + Selection.activeGameObject.transform.position.y);
            Debug.Log(Selection.activeGameObject.name + " z is at " + Selection.activeGameObject.transform.position.z);
        }
    }
}