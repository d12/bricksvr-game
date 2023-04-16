using UnityEngine;

public static class PlacedBrickCreator
{
    private static SessionManager _sessionManager;

    public static GameObject CreateFromBrickObject(NormcoreRPC.Brick brick, bool recalculateMesh = true)
    {
        return CreateFromAttributes(brick.matId, brick.type, brick.pos, brick.rot, brick.uuid, brick.color, brick.usingNewColor, brick.usingHeadStuff ? brick.headClientId : -1, recalculateMesh);
    }

    private static GameObject CreateFromAttributes(int matId, string type, Vector3 pos, Quaternion rot, string uuid, int color, bool usingNewColor, int headClientId, bool recalculateMesh = true)
    {
        GameObject brickObject;
        if (headClientId == -1)
        {
            brickObject = GameObject.Instantiate(BrickPrefabCache.GetInstance().Get($"{type} - Placed"), pos, rot);
        }
        else
        {
            AvatarManager avatarManager = AvatarManager.GetInstance();
            if (!avatarManager.avatars.ContainsKey(headClientId))
            {
                Debug.LogError("Got a brick parented to a user that no longer exists!");
                return null;
            }

            Transform headTransform = avatarManager.avatars[headClientId].head;

            brickObject = GameObject.Instantiate(BrickPrefabCache.GetInstance().Get($"{type} - Placed"), headTransform);
            brickObject.transform.localPosition = pos;
            brickObject.transform.localRotation = rot;
        }
        BrickAttach newBrickAttach = brickObject.GetComponent<BrickAttach>();
        newBrickAttach.headClientId = headClientId;

        newBrickAttach.Color = usingNewColor ? ColorInt.IntToColor32(color) : BrickColorMap.ColorFromID(matId);

        BrickUuid brickUuid = brickObject.GetComponent<BrickUuid>();
        brickUuid.uuid = uuid;

        BrickStore.GetInstance().Put(uuid, brickObject);

        BrickMeshRecalculator.GetInstance().AddAttach(newBrickAttach);

        if (_sessionManager == null)
            _sessionManager = SessionManager.GetInstance();

        BrickSounds sounds = BrickSounds.GetInstance();

        if (!_sessionManager.Loading() && UserSettings.GetInstance().BrickClickSoundsEnabled)
        {
            if (newBrickAttach.IsOnCarpet())
            {
                sounds.PlayBrickCarpetSound(pos);
            }
            else
            {
                sounds.PlayBrickSnapSound(pos);
            }
        }

        if(recalculateMesh)
            newBrickAttach.NotifyNearbyBricksToRecalculateMesh();

        return brickObject;
    }

    public static void DestroyBrickObject(GameObject gameObject) {
        SessionManager sessionManager = SessionManager.GetInstance();
        Session session = sessionManager.session;

        if(session.isSinglePlayer)
            GameObject.Destroy(gameObject);
    }
}
