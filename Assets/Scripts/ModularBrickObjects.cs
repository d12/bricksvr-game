using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularBrickObjects : MonoBehaviour
{

    private static ModularBrickObjects _instance;

    public static ModularBrickObjects GetInstance()
    {
        if (_instance == null) _instance = FindObjectOfType<ModularBrickObjects>();
        return _instance;
    }

    private const int HollowBrick = 0;
    private const string HollowBrickNameInModel = "Body";

    private const int SolidBrick = 1;
    private const string SolidBrickNameInModel = "FlatBody";

    private const string StudNameInModel = "Studs";

    private readonly Dictionary<string, GameObject> _namesToModularModels = new Dictionary<string, GameObject>();
    private readonly Dictionary<(string, int), Mesh> _namesAndHollowToMesh = new Dictionary<(string, int), Mesh>();
    private readonly Dictionary<string, Mesh> _namesToStudMeshes = new Dictionary<string, Mesh>();
    private readonly Dictionary<(string, string), Vector3> _namesAndStudNamesToStudOffset = new Dictionary<(string, string), Vector3>();

    public GameObject GetModularModel(string brickName)
    {
        if (!_namesToModularModels.ContainsKey(brickName))
            _namesToModularModels.Add(brickName, transform.Find(brickName).gameObject);

        return _namesToModularModels[brickName];
    }

    public Mesh GetHollowMesh(string brickName)
    {
        if (!_namesAndHollowToMesh.ContainsKey((brickName, HollowBrick)))
        {
            _namesAndHollowToMesh[(brickName, HollowBrick)] = GetModularModel(brickName).transform
                .Find(HollowBrickNameInModel).GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        return _namesAndHollowToMesh[(brickName, HollowBrick)];
    }

    public Mesh GetSolidMesh(string brickName)
    {
        if (!_namesAndHollowToMesh.ContainsKey((brickName, SolidBrick)))
        {
            _namesAndHollowToMesh[(brickName, SolidBrick)] = GetModularModel(brickName).transform.Find(SolidBrickNameInModel).GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        return _namesAndHollowToMesh[(brickName, SolidBrick)];
    }

    public Mesh GetStudMesh(string brickName)
    {
        if (!_namesToStudMeshes.ContainsKey(brickName))
        {
            _namesToStudMeshes[brickName] = GetModularModel(brickName).transform
                .Find(StudNameInModel)?.GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        return _namesToStudMeshes[brickName];
    }

    public Vector3 GetStudOffsetFromCenter(string brickName, string studName)
    {
        if (!_namesAndStudNamesToStudOffset.ContainsKey((brickName, studName)))
        {
            Transform modularModel = GetModularModel(brickName).transform;
            Vector3 rootPos = modularModel.position;
            Vector3 studPos =
                _namesAndStudNamesToStudOffset[(brickName, studName)] = modularModel
                    .Find(StudNameInModel).Find(studName).position;

            _namesAndStudNamesToStudOffset[(brickName, studName)] = rootPos - studPos;
        }

        return _namesAndStudNamesToStudOffset[(brickName, studName)];
    }
}
