using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;


public static class PrintBricks
{
    [MenuItem("Debug/Print all brick data as JSON")]
    public static void PrintBrickData()
    {
        BrickAttach[] brickAttaches = GameObject.FindObjectsOfType<BrickAttach>();
        Brick[] bricks = brickAttaches.Select(a => new Brick()
        {
            uuid = a.GetUuid(),
            matId = BrickColorMap.IDFromColor(a.Color),
            type = a.normalPrefabName,
            pos = a.gameObject.transform.position,
            rot = a.gameObject.transform.rotation
        }).ToArray();

        BrickCollectionJson jsonObject = new BrickCollectionJson()
            {bricks = bricks, room = SessionManager.GetInstance().session.name};

        string path = $"Assets/Resources/{SessionManager.GetInstance().session.name}";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(JsonUtility.ToJson(jsonObject));
        writer.Close();

        Debug.Log($"Saved data to {path}");
    }

    public class BrickCollectionJson
    {
        public Brick[] bricks;
        public string room;
    }

    [Serializable]
    public class Brick
    {
        public string uuid;
        public int matId;
        public string type;
        public Vector3 pos;
        public Quaternion rot;
    }
}