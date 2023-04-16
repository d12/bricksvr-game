using UnityEngine;
using System;

public class NormcoreRPC : MonoBehaviour
{
    [Serializable]
    public class RPCMessage
    {
        public string type;
        public Brick brick;
    }

    [Serializable]
    public class Brick
    {
        public string uuid;
        public int matId;
        public int color;
        public string type;
        public Vector3 pos;
        public Quaternion rot;
        public bool usingNewColor;
        public int headClientId;
        public bool usingHeadStuff;
    }
}
