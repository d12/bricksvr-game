using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildCompleteMeshesNoBevel : MonoBehaviour
{
    public bool run;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!run) return;
        run = false;

        foreach (Transform child in transform)
        {
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            foreach(MeshFilter meshFilter in child.GetComponentsInChildren<MeshFilter>())
            {
                GameObject obj = meshFilter.gameObject;
                string parentName = meshFilter.transform.parent.name;

                switch (parentName)
                {
                    case "FlatBody":
                        continue;
                }

                CombineInstance instance = new CombineInstance
                {
                    mesh = obj.GetComponent<MeshFilter>().sharedMesh,
                    transform = obj.GetComponent<MeshFilter>().transform.localToWorldMatrix
                };
                combineInstances.Add(instance);
            }

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combineInstances.ToArray());
            newMesh.Optimize();

            AssetDatabase.CreateAsset(newMesh, $"Assets/Resources/BrickModels/CompleteNoBevel/{child.name}.asset");
            break;
        }

        AssetDatabase.SaveAssets();
    }
    #endif
}
