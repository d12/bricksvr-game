using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.Networking;

public static class BrickSwapper
{
    public static GameObject SwapToRealBrick(GameObject brick)
    {
        if (brick.GetComponent<RealtimeView>() != null) return null; // Brick is already a real brick lol

        BrickAttach brickAttach = brick.GetComponent<BrickAttach>();
        if (!brickAttach)
            return null;

        Transform t = brick.transform;

        // NormcoreRPC.Brick serializedBrickObject = new NormcoreRPC.Brick()
        // {
        //     uuid = brickAttach.GetUuid(),
        // };
        // DestroyBrickOverRPC(serializedBrickObject);

        // Remove the brick from the datastore since the datastore specifically stores PLACED bricks
        BrickServerInterface.GetInstance().RemoveBrick(brickAttach.GetUuid(), Realtime.instances.First());

        GameObject newBrick = Realtime.Instantiate(
            brickAttach.swapPrefab,
            t.position,
            t.rotation,
            ownedByClient: false,
            preventOwnershipTakeover: false,
            destroyWhenOwnerOrLastClientLeaves: true,
            useInstance: null);
        BuildingBrickSync newBrickSync = newBrick.GetComponent<BuildingBrickSync>();
        // newBrickSync.CopyFieldsFromSync(brick.GetComponent<BuildingBrickSync>());

        newBrickSync.EnableNewColors();
        newBrickSync.SetColor(ColorInt.Color32ToInt(brickAttach.Color));
        newBrickSync.SetUuid(brickAttach.GetUuid());
        newBrickSync.SetAttached(false);

        newBrick.GetComponent<BrickAttach>().texOffset = brickAttach.texOffset;

        // Gets destroyed by the UUID setter in BuildingBrickSync because of the duplicate uuid
        //brickAttach.DelayedDestroy();
        brick.SetActive(false);

        return newBrick;
    }

    public static GameObject SwapToFakeBrick(GameObject brick, int headClientId = -1, RealtimeAvatarManager avatarManager = null)
    {
        if (brick.GetComponent<RealtimeView>() == null) return null; // Brick is already fake

        BrickAttach brickAttach = brick.GetComponent<BrickAttach>();

        NormcoreRPC.Brick serializedBrickObject = new NormcoreRPC.Brick()
        {
            color = ColorInt.ColorToInt(brickAttach.Color),
            type = brickAttach.normalPrefabName,
            uuid = brickAttach.GetUuid(),
            pos = brick.transform.position,
            rot = brick.transform.rotation,
            usingNewColor = true,
            headClientId = headClientId,
            usingHeadStuff = true,
        };

        // If this brick is on a head, send the relative position/rotation instead of the world position/rotation
        if (headClientId != -1)
        {
            brick.transform.parent = avatarManager.avatars[headClientId].head;
            serializedBrickObject.pos = brick.transform.localPosition;
            serializedBrickObject.rot = brick.transform.localRotation;
        }

        Realtime realtime = Realtime.instances.First();

        if (!TutorialManager.GetInstance().IsTutorialRunning())
        {
            CreateBrickOverRPC(serializedBrickObject, realtime);
            BrickServerInterface.GetInstance().SendBrick(serializedBrickObject, realtime);
        }

        GameObject newBrick = PlacedBrickCreator.CreateFromBrickObject(serializedBrickObject);
        newBrick.GetComponent<BrickAttach>().texOffset = brickAttach.texOffset;

        brickAttach.DelayedDestroy();

        return newBrick;
    }

    private static void CreateBrickOverRPC(NormcoreRPC.Brick brick, Realtime realtime)
    {
        NormcoreRPC.RPCMessage rpcMessage = new NormcoreRPC.RPCMessage()
        {
            type = "create",
            brick = brick
        };

        SendMessageOverRPC(rpcMessage, realtime);
    }

    private static void DestroyBrickOverRPC(NormcoreRPC.Brick brick, Realtime realtime)
    {
        NormcoreRPC.RPCMessage rpcMessage = new NormcoreRPC.RPCMessage()
        {
            type = "destroy",
            brick = brick
        };

        SendMessageOverRPC(rpcMessage, realtime);
    }

    private static void SendMessageOverRPC(NormcoreRPC.RPCMessage rpcMessage, Realtime realtime)
    {
        string message = JsonUtility.ToJson(rpcMessage);
        message = $"{message.Length}@{message}";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

        // Debug.Log($"Sending message over RPC: {message}");
        realtime.room.SendRPCMessage(bytes, bytes.Length, true);
    }
}
