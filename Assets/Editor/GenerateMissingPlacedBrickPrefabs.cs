using UnityEditor;
using UnityEngine;

public static class GenerateMissingPlacedBrickPrefabs
{
    [MenuItem("Generator/Generate placed brick object for brick")]
    private static void GeneratePlacedBrick()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("No brick selected");
            return;
        }

        GameObject originalBrick = Selection.activeGameObject;
        if (!originalBrick.GetComponent<BrickAttach>())
        {
            Debug.LogError("Selected brick is not a valid unplaced brick.");
            return;
        }

        string prefabName = Selection.activeGameObject.name;
        string newPrefabName = $"{prefabName} - Placed";

        if (DoesBrickExist(newPrefabName))
        {
            Debug.LogError("We already have a placed brick for that brick!");
            return;
        }

        GameObject blankBrick = GetBlankBrickTemplate();

        GameObject newBrick = GameObject.Instantiate(blankBrick);

        // MODULAR MODEL
        // Transform originalModularModel = originalBrick.transform.Find("ModularModel");
        // Transform newModularModel = newBrick.transform.Find("ModularModel");
        //
        // Transform originalTubesObj = originalModularModel.Find("Tubes");
        // if (originalTubesObj != null)
        // {
        //     Transform newTubesWrapper = newModularModel.Find("Tubes");
        //     foreach (Transform originalTube in originalTubesObj)
        //     {
        //         GameObject newTube = new GameObject(originalTube.name);
        //         newTube.transform.parent = newTubesWrapper;
        //         newTube.transform.localPosition = originalTube.transform.localPosition;
        //         newTube.transform.localRotation = originalTube.transform.localRotation;
        //         newTube.transform.localScale = originalTube.transform.localScale;
        //
        //         MeshFilter mf = newTube.AddComponent<MeshFilter>();
        //         mf.sharedMesh = originalTube.GetComponent<MeshFilter>().sharedMesh;
        //     }
        // }
        // else
        // {
        //     GameObject.DestroyImmediate(newModularModel.Find("Tubes").gameObject);
        // }
        //
        // Transform originalStudsObj = originalModularModel.Find("Studs");
        // if (originalStudsObj != null)
        // {
        //     Transform newStudsWrapper = newModularModel.Find("Studs");
        //     newStudsWrapper.localPosition = originalStudsObj.localPosition;
        //     foreach (Transform originalStud in originalStudsObj)
        //     {
        //         GameObject newStud = new GameObject(originalStud.name);
        //         newStud.transform.parent = newStudsWrapper;
        //         newStud.transform.localPosition = originalStud.transform.localPosition;
        //         newStud.transform.localRotation = originalStud.transform.localRotation;
        //         newStud.transform.localScale = originalStud.transform.localScale;
        //
        //         MeshFilter mf = newStud.AddComponent<MeshFilter>();
        //         mf.sharedMesh = originalStud.GetComponent<MeshFilter>().sharedMesh;
        //     }
        // }
        // else
        // {
        //     GameObject.DestroyImmediate(newModularModel.Find("Tubes").gameObject);
        // }
        //
        // Transform originalBodyWrapper = originalModularModel.Find("Body");
        // if (originalBodyWrapper != null)
        // {
        //     Transform newBodyWrapper = newModularModel.Find("Body");
        //     Transform newBodyObj = newBodyWrapper.GetChild(0);
        //     newBodyObj.gameObject.GetComponent<MeshFilter>().sharedMesh =
        //         originalBodyWrapper.GetComponentInChildren<MeshFilter>().sharedMesh;
        // }
        //
        // Transform originalFlatBodyWrapper = originalModularModel.Find("FlatBody");
        // if (originalFlatBodyWrapper != null)
        // {
        //     Transform newFlatBodyWrapper = newModularModel.Find("FlatBody");
        //     Transform newFlatBodyObj = newFlatBodyWrapper.GetChild(0);
        //     newFlatBodyObj.gameObject.GetComponent<MeshFilter>().sharedMesh =
        //         originalFlatBodyWrapper.GetComponentInChildren<MeshFilter>().sharedMesh;
        // }

        newBrick.transform.Find("CombinedModel").gameObject.GetComponent<MeshFilter>().sharedMesh =
            originalBrick.transform.Find("CombinedModel").GetComponent<MeshFilter>().sharedMesh;

        // Connectors
        GameObject originalMaleConnectorParent = originalBrick.transform.Find("MaleConnectors").gameObject;
        GameObject newMaleConnectorParent = newBrick.transform.Find("MaleConnectors").gameObject;

        newMaleConnectorParent.transform.localPosition = originalMaleConnectorParent.transform.localPosition;
        newMaleConnectorParent.transform.localRotation = originalMaleConnectorParent.transform.localRotation;
        newMaleConnectorParent.transform.localScale = originalMaleConnectorParent.transform.localScale;

        foreach (Transform child in originalMaleConnectorParent.transform)
        {
            if (child == null) continue;

            GameObject newConnector = new GameObject(child.name);
            newConnector.transform.parent = newMaleConnectorParent.transform;

            newConnector.transform.localPosition = child.localPosition;
            newConnector.transform.localRotation = child.localRotation;
            newConnector.transform.localScale = child.localScale;

            newConnector.layer = LayerMask.NameToLayer("placed connector");
            newConnector.tag = "Connector-Male";

            BoxCollider b = newConnector.AddComponent<BoxCollider>();
            b.center = child.GetComponent<BoxCollider>().center;
            b.size = child.GetComponent<BoxCollider>().size;

            LegoConnectorScript script = newConnector.AddComponent<LegoConnectorScript>();
            script.female = child.GetComponent<LegoConnectorScript>().female;
            script.placed = true;
        }

        GameObject originalFemaleConnectorParent = originalBrick.transform.Find("FemaleConnectors").gameObject;
        GameObject newFemaleConnectorParent = newBrick.transform.Find("FemaleConnectors").gameObject;

        newFemaleConnectorParent.transform.localPosition = originalFemaleConnectorParent.transform.localPosition;
        newFemaleConnectorParent.transform.localRotation = originalFemaleConnectorParent.transform.localRotation;
        newFemaleConnectorParent.transform.localScale = originalFemaleConnectorParent.transform.localScale;

        foreach (Transform child in originalFemaleConnectorParent.transform)
        {
            if (child == null) continue;

            GameObject newConnector = new GameObject(child.name);
            newConnector.transform.parent = newFemaleConnectorParent.transform;

            newConnector.transform.localPosition = child.localPosition;
            newConnector.transform.localRotation = child.localRotation;
            newConnector.transform.localScale = child.localScale;

            newConnector.layer = LayerMask.NameToLayer("placed connector");
            newConnector.tag = "Connector-Female";

            BoxCollider b = newConnector.AddComponent<BoxCollider>();
            b.center = child.GetComponent<BoxCollider>().center;
            b.size = child.GetComponent<BoxCollider>().size;

            LegoConnectorScript script = newConnector.AddComponent<LegoConnectorScript>();
            script.female = child.GetComponent<LegoConnectorScript>().female;
            script.placed = true;
        }

        // Box collider
        BoxCollider newCollider = newBrick.GetComponent<BoxCollider>();
        BoxCollider oldCollider = originalBrick.GetComponent<BoxCollider>();

        newCollider.center = oldCollider.center;
        newCollider.size = oldCollider.size;
        newCollider.enabled = oldCollider.enabled;
        newCollider.isTrigger = oldCollider.isTrigger;

        // Other colliders
        Transform otherCollidersParentTransform = originalBrick.transform.Find("ExtraColliders");
        if (otherCollidersParentTransform != null)
        {
            GameObject otherCollidersParent = otherCollidersParentTransform.gameObject;
            GameObject newCollidersParent = new GameObject("Extra Colliders");
            newCollidersParent.transform.parent = newBrick.transform;
            newCollidersParent.transform.localPosition = otherCollidersParentTransform.localPosition;
            newCollidersParent.transform.localRotation = otherCollidersParentTransform.localRotation;
            newCollidersParent.layer = LayerMask.NameToLayer("placed lego");

            foreach (Transform child in otherCollidersParent.transform)
            {
                GameObject newExtraColliderObject = new GameObject(child.name);
                newExtraColliderObject.layer = LayerMask.NameToLayer("placed lego");
                newExtraColliderObject.transform.parent = newCollidersParent.transform;
                newExtraColliderObject.transform.localPosition = child.localPosition;
                newExtraColliderObject.transform.localRotation = child.localRotation;

                BoxCollider otherCollider = child.GetComponent<BoxCollider>();
                BoxCollider b = newExtraColliderObject.AddComponent<BoxCollider>();
                b.center = otherCollider.center;
                b.size = otherCollider.size;
            }
        }

        // Brick Attach
        BrickAttach attach = newBrick.GetComponent<BrickAttach>();
        attach.swapPrefab = prefabName;
        attach.normalPrefabName = prefabName;

        // Save object
        string path = $"Assets/Resources/{newPrefabName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(newBrick, path, out bool success);

        GameObject.DestroyImmediate(newBrick);

        Debug.Log($"Successfuly saved: {success}");
    }

    private static GameObject GetBlankBrickTemplate()
    {
        return Resources.Load<GameObject>("BlankPlacedBrick");
    }

    private static bool DoesBrickExist(string name)
    {
        return Resources.Load<GameObject>(name) != null;
    }
}
