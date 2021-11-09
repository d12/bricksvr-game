using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class NormcoreRPC : MonoBehaviour
{
    public Realtime _realtime;

    private void Start()
    {
        _realtime.didConnectToRoom += (realtime) =>
        {
            realtime.room.rpcMessageReceived += RpcMessageReceived;
            Debug.Log("Registered for RPC events");
        };
    }

    private void RpcMessageReceived(Room room, int senderId, byte[] data, bool reliable)
    {
        string stringMessage =  System.Text.Encoding.UTF8.GetString(data);

        // Sometimes the RPC message we get has garbage on the end. Find the last } in the message and use that to trim.
        // Also, pray the garbage char at the end isn't }
        int separatorIndex = stringMessage.IndexOf("@", StringComparison.Ordinal);
        int messageLength = int.Parse(stringMessage.Substring(0, stringMessage.IndexOf("@", StringComparison.Ordinal)));
        string trimmedMessage = stringMessage.Substring(separatorIndex + 1, messageLength);

        RPCMessage message = JsonUtility.FromJson<RPCMessage>(trimmedMessage);
        if (message.type == "create")
        {
            PlacedBrickCreator.CreateFromBrickObject(message.brick);
        } else if (message.type == "destroy")
        {

        }
    }

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
