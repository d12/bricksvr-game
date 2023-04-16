using UnityEngine;
using System.Linq;

public static class BrickSwapper
{
    public static GameObject SwapToRealBrick(GameObject brick)
    {
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
        BrickServerInterface.GetInstance().RemoveBrick(brickAttach.GetUuid());

        GameObject newBrick = GameObject.Instantiate(
            Resources.Load<GameObject>(brickAttach.swapPrefab),
            t.position,
            t.rotation
        );

        BrickAttach attach = newBrick.GetComponent<BrickAttach>();
        attach.Color = (brickAttach.Color);
        attach.SetUuid(brickAttach.GetUuid());

        newBrick.GetComponent<BrickAttach>().texOffset = brickAttach.texOffset;

        // Gets destroyed by the UUID setter in BuildingBrickSync because of the duplicate uuid
        //brickAttach.DelayedDestroy();
        brick.SetActive(false);

        return newBrick;
    }

    public static GameObject SwapToFakeBrick(GameObject brick, int headClientId = -1, AvatarManager avatarManager = null)
    {
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

        if (!TutorialManager.GetInstance().IsTutorialRunning())
        {
            BrickServerInterface.GetInstance().SendBrick(serializedBrickObject);
        }

        GameObject newBrick = PlacedBrickCreator.CreateFromBrickObject(serializedBrickObject);
        newBrick.GetComponent<BrickAttach>().texOffset = brickAttach.texOffset;

        brickAttach.DelayedDestroy();

        return newBrick;
    }
}
