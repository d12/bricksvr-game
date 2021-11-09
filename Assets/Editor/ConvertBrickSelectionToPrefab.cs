using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ConvertBrickSelectionToPrefab
{
    [MenuItem("Generator/Convert selected bricks to prefab")]
    public static void ConvertToPrefab()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        List<GameObject> bricks = GetBricksFromSelection(selectedObjects);
        if (bricks.Count == 0)
        {
            Debug.LogError("No bricks selected!");
            return;
        }

        GameObject newPrefab = new GameObject();
        foreach (GameObject brick in bricks)
        {
            BrickAttach attach = brick.GetComponent<BrickAttach>();

            GameObject brickObject = new GameObject();
            brickObject.transform.localPosition = brick.transform.localPosition;
            brickObject.transform.localRotation = brick.transform.localRotation;
            brickObject.transform.localScale = attach.model.transform.localScale;
            brickObject.transform.parent = newPrefab.transform;

            MeshFilter mf = brickObject.AddComponent<MeshFilter>();
            mf.mesh = attach.originalMesh;

            MeshRenderer mr = brickObject.AddComponent<MeshRenderer>();
            mr.material = attach.originalMaterial;

            SetColorOnPrefabBrick colorSetter = brickObject.AddComponent<SetColorOnPrefabBrick>();
            colorSetter.color = attach.Color;
        }

        string prefabName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/NewPrefabFromScene.prefab");
        PrefabUtility.SaveAsPrefabAssetAndConnect(newPrefab, prefabName, InteractionMode.UserAction);

        GameObject.Destroy(newPrefab);

        Debug.Log($"Saved prefab as {prefabName}");
    }

    private static List<GameObject> GetBricksFromSelection(GameObject[] objects)
    {
        List<GameObject> bricks = new List<GameObject>();
        foreach (GameObject o in objects)
        {
            if (o.name == "CombinedModel")
            {
                bricks.Add(o.transform.parent.gameObject);
                continue;
            }

            if (o.GetComponent<BrickAttach>())
            {
                bricks.Add(o);
            }
        }

        return bricks;
    }
}
