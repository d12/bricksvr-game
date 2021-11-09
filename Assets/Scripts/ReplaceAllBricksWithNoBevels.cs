using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceAllBricksWithNoBevels : MonoBehaviour
{
    public bool replace;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!replace) return;
        replace = false;

        Dictionary<string, Mesh> cachedMeshes = new Dictionary<string, Mesh>();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in meshFilters)
        {
            BrickData.Brick brick = BrickData.BrickByCompleteMeshName(mf.sharedMesh.name.Split(' ')[0]);

            if (cachedMeshes.ContainsKey(brick.PrefabName))
            {
                mf.mesh = cachedMeshes[brick.PrefabName];
                continue;
            }

            GameObject modularModel = ModularBrickObjects.GetInstance().GetModularModel(brick.PrefabName);

            List<CombineInstance> combineInstances = new List<CombineInstance>();
            foreach(MeshFilter meshFilter in modularModel.GetComponentsInChildren<MeshFilter>())
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
            cachedMeshes[brick.PrefabName] = newMesh;
            mf.mesh = newMesh;
        }
    }
    #endif
}
