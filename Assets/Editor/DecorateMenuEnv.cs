using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

public static class DecorateMenuEnv
{
    static List<GameObject> bricks;
    static List<Material> colors;

    static GameObject startPoint;
    static GameObject floorDecorationsParent;

    // We'll spawn bricks between innerBounds and outerBounds distance from the startPoint
    static float innerBounds = 10f;
    static float outerBounds = 13f;

    static float lowerHeight = 3f;
    static float upperHeight = 50f;

    // How many steps to process physics for after creating bricks. Bump this up if bricks aren't settled after decorating.
    static int physicsDuration = 5000;

    [MenuItem("Helpers/Decorate Menu Space")]
    public static void DecorateMenu()
    {
        startPoint = GameObject.Find("MenuPlayerSpawnPoint");
        floorDecorationsParent = GameObject.Find("MenuFloorDecorations");

        LoadBrickPrefabs();
        LoadBrickColors();

        SpawnBricks(500);

        for (int i = 0; i < physicsDuration; i++)
        {
            // StepPhysics();
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    static void SpawnBricks(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject brick = bricks[Random.Range(0, bricks.Count())];
            Material mat = colors[Random.Range(0, colors.Count())];

            GameObject newBrick = PrefabUtility.InstantiatePrefab(brick) as GameObject;
            PrefabUtility.UnpackPrefabInstance(newBrick, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            newBrick.transform.parent = floorDecorationsParent.transform;

            newBrick.transform.localPosition = RandomBrickPos();

            // Destroy realtime components that we don't need.
            GameObject.DestroyImmediate(newBrick.GetComponent<RealtimeTransform>());
            GameObject.DestroyImmediate(newBrick.GetComponent<RealtimeView>());
            GameObject.DestroyImmediate(newBrick.GetComponent<BrickAttach>());
            GameObject.DestroyImmediate(newBrick.GetComponent<BuildingBrickSync>());
            GameObject.DestroyImmediate(newBrick.GetComponent<Outline>());
            GameObject.DestroyImmediate(newBrick.GetComponent<BrickUuid>());
            GameObject.DestroyImmediate(newBrick.GetComponent<XRGrabInteractable>());
            GameObject.DestroyImmediate(newBrick.GetComponent<BrickSounds>());
            GameObject.DestroyImmediate(newBrick.GetComponent<BrickAttachDetector>());
            GameObject.DestroyImmediate(newBrick.GetComponent<ShowSnappableBrickPositions>());
            GameObject.DestroyImmediate(newBrick.GetComponent<SelectionBase>());

            GameObject.DestroyImmediate(newBrick.transform.Find("MaleConnectors").gameObject);
            GameObject.DestroyImmediate(newBrick.transform.Find("FemaleConnectors").gameObject);
            GameObject.DestroyImmediate(newBrick.transform.Find("grip").gameObject);

            newBrick.AddComponent<FallingBrick>();

            MeshRenderer renderer = newBrick.GetComponentInChildren<MeshRenderer>();
            renderer.material = mat;

            Rigidbody rb = newBrick.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.drag = 6f;
            rb.angularDrag = 0f;
        }
    }

    static void LoadBrickPrefabs()
    {
        bricks = new List<GameObject>();
        bricks.Add((GameObject)Resources.Load("4x2"));
        bricks.Add((GameObject)Resources.Load("2x2"));
        bricks.Add((GameObject)Resources.Load("1x4"));
    }

    static void LoadBrickColors()
    {
        string[] matGUIDs = AssetDatabase.FindAssets(null, new string[] { "Assets/Resources/BrickMaterials" });
        List<string> matPaths = new List<string>(matGUIDs.Select(guid => AssetDatabase.GUIDToAssetPath(guid)));
        matPaths.RemoveAll(path => path.Contains("ghost")); // Remove ghost mat
        Material[] materials = matPaths.Select(path => AssetDatabase.LoadAssetAtPath<Material>(path)).ToArray();
        // Material m = Resources.Load<Material>("BrickMaterials/bright-blue");
        colors = new List<Material>(materials);
    }

    static Vector3 RandomBrickPos()
    {
        while (true)
        {
            float x = Random.Range(0f, outerBounds);
            if (Random.Range(0f, 1f) > 0.5f)
                x *= -1;

            float z = Random.Range(0f, outerBounds);
            if (Random.Range(0f, 1f) > 0.5f)
                z *= -1;

            float y = Random.Range(lowerHeight, upperHeight);

            Vector3 newPos = new Vector3(x, y, z);
            Vector3 newPosZeroedY = new Vector3(x, 0, z);

            if (Vector3.Distance(Vector3.zero, newPosZeroedY) > innerBounds && Vector3.Distance(Vector3.zero, newPosZeroedY) < outerBounds)
            {
                return newPos;
            }
        }
    }

    static void StepPhysics()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.autoSimulation = true;
    }
}