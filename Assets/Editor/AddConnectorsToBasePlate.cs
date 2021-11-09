using UnityEngine;
using UnityEditor;

public static class AddConnectorsToBasePlate
{
    const float SPACE_BETWEEN_PEGS = 0.05607514f;

    [MenuItem("Helpers/Add Connectors to Base Plate")]
    public static void AddConnectors()
    {
        GameObject basePlate = GameObject.Find("Base Plate");
        Transform maleConnectorParentTransform = basePlate.transform.Find("MaleConnectors");
        GameObject maleConnectorParent = maleConnectorParentTransform.gameObject;

        GameObject model = basePlate.transform.Find("model").gameObject;

        Mesh cubeMesh = getCubeMesh();
        Material redMaterial = getRedMaterial();

        int dimensions = 32;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                // i/j go from 0 -> 32. Convert this to go from -16 to +16
                // Add the 0.5f because it makes the whole thing work :)
                float x = i - ((float)dimensions / 2) + 0.5f;
                float z = j - ((float)dimensions / 2) + 0.5f;

                GameObject newConnector = new GameObject(i + "," + j);
                newConnector.tag = "Connector-Male";
                newConnector.transform.SetParent(maleConnectorParentTransform);
                newConnector.transform.position = maleConnectorParentTransform.position + new Vector3(x * SPACE_BETWEEN_PEGS, 0.023f, z * SPACE_BETWEEN_PEGS);
                newConnector.transform.localScale = new Vector3(0.056f, 0.04f, 0.056f);

                MeshFilter meshFilter = newConnector.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = cubeMesh;

                MeshRenderer meshRenderer = newConnector.AddComponent<MeshRenderer>();
                meshRenderer.material = redMaterial;
                meshRenderer.enabled = false;

                LegoConnectorScript connectorScript = newConnector.AddComponent<LegoConnectorScript>();

                BoxCollider collider = newConnector.AddComponent<BoxCollider>();
                collider.isTrigger = true;
            }
        }

        Debug.Log("Done!");
    }

    private static Mesh getCubeMesh()
    {
        Mesh[] meshes = Resources.FindObjectsOfTypeAll<Mesh>();

        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i].name == "Cube")
            {
                return meshes[i];
            }
        }

        return null;
    }

    private static Material getRedMaterial()
    {
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].name == "Red")
            {
                return materials[i];
            }
        }

        return null;
    }
}