using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To use, add component to an object you want to align, and set connector attribute.
// Click the "align" checkbox in the component attributes to align.
public class AlignToBrickGrid : MonoBehaviour
{
    private const float X_BRICK_INTERVAL = 0.0560751f;
    private const float Y_BRICK_INTERVAL = 0.067136f;
    private const float Z_BRICK_INTERVAL = 0.0560751f;

    public bool align;

    void OnValidate()
    {
        if (!align) return;

        LegoConnectorScript[] connectorScripts = GetComponentsInChildren<LegoConnectorScript>();
        if (connectorScripts.Length == 0)
        {
            Debug.Log("Object has no connectors, so we cannot align it.");
            align = false;
            return;
        }

        GameObject connector = connectorScripts[0].gameObject;

        Vector3 originalPos = connector.transform.position;
        Vector3 newPos = connector.transform.position;
        newPos.x = Mathf.Round(newPos.x / X_BRICK_INTERVAL) * X_BRICK_INTERVAL;
        newPos.y = Mathf.Round(newPos.y / Y_BRICK_INTERVAL) * Y_BRICK_INTERVAL;
        newPos.z = Mathf.Round(newPos.z / Z_BRICK_INTERVAL) * Z_BRICK_INTERVAL;

        Vector3 difference = newPos - originalPos;

        if (difference != Vector3.zero)
        {
            transform.position += difference;
            Debug.Log("Aligned object with grid");
        }
        else
        {
            Debug.Log("Object already aligned");
        }

        Debug.Log("Objects grid position is (" +
                  (newPos.x / X_BRICK_INTERVAL) + "," +
                  (newPos.y / Y_BRICK_INTERVAL) + "," +
                  (newPos.z / Z_BRICK_INTERVAL) + ")");

        Debug.Log(connector.transform.position.x);

        align = false;
    }
}
