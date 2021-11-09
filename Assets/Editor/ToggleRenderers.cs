using UnityEngine;
using UnityEditor;

public static class ToggleRenderers
{
    [MenuItem("Helpers/ENABLE renderers in selection")]
    public static void EnableRenderers()
    {
        GameObject obj = Selection.activeGameObject;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = true;
    }

    [MenuItem("Helpers/DISABLE renderers in selection")]
    public static void DisableRenderers()
    {
        GameObject obj = Selection.activeGameObject;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = false;
    }

    [MenuItem("Helpers/DISABLE colliders in selection")]
    public static void DisableColliders()
    {
        GameObject obj = Selection.activeGameObject;
        Collider[] renderers = obj.GetComponentsInChildren<Collider>();
        foreach (Collider r in renderers)
            r.enabled = false;
    }


}