using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

public static class MiscFixup
{
    [MenuItem("Fixup/Misc Fixup")]
    public static void Fixup()
    {
        List<BrickData.Brick> bricks = BrickData.AllBricks();
        foreach (BrickData.Brick brick in bricks)
        {
            string path = $"Assets/Resources/{brick.PrefabName}.prefab";

            // Modify prefab contents and save it back to the Prefab Asset
            using (EditPrefabAssetScope editScope = new EditPrefabAssetScope(path))
            {
                GameObject obj = editScope.prefabRoot;

                obj.transform.Find("CombinedModel").GetComponent<MeshRenderer>().receiveShadows = true;
                obj.transform.Find("CombinedModel").GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }

            path = $"Assets/Resources/{brick.PrefabName} - Placed.prefab";

            // Modify prefab contents and save it back to the Prefab Asset
            using (EditPrefabAssetScope editScope = new EditPrefabAssetScope(path))
            {
                GameObject obj = editScope.prefabRoot;

                obj.transform.Find("CombinedModel").GetComponent<MeshRenderer>().receiveShadows = true;
                obj.transform.Find("CombinedModel").GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }
}