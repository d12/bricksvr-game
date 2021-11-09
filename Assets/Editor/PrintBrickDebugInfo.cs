using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEditor;
using UnityEngine;

public static class PrintBrickDebugInfo
{
    // Start is called before the first frame update
    [MenuItem("Debug/Print Brick Debug Info")]
    public static void PrintBrickInfo()
    {
        BrickAttach[] attaches = (BrickAttach[]) GameObject.FindObjectsOfType (typeof(BrickAttach));
        List<GameObject> freeBricks = new List<GameObject>();
        List<GameObject> attachedBricks = new List<GameObject>();

        foreach (BrickAttach attach in attaches)
        {
            GameObject obj = attach.gameObject;
            if (obj.GetComponent<RealtimeTransform>() != null)
            {
                freeBricks.Add(obj);
            }
            else
            {
                attachedBricks.Add(obj);
            }
        }

        string debugString = "Info:\n";
        debugString += "TOTAL BRICKS: " + (freeBricks.Count + attachedBricks.Count) + "\n";
        debugString += "FREE BRICKS: " + freeBricks.Count + "\n";
        debugString += "ATTACHED BRICKS: " + attachedBricks.Count + "\n";

        Debug.Log(debugString);
    }
}