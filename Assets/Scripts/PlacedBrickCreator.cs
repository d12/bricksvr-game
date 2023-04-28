using UnityEngine;

public static class PlacedBrickCreator
{
    private static SessionManager _sessionManager;

    public static GameObject CreateFromBrickObject(BrickData.LocalBrickData brick, bool recalculateMesh = true)
    {
        return CreateFromAttributes(brick.type, BrickData.CustomVec3.To(brick.pos), BrickData.CustomQuaternion.To(brick.rot), brick.color, recalculateMesh);
    }

    private static GameObject CreateFromAttributes(string type, Vector3 pos, Quaternion rot, int color, bool recalculateMesh = true, int headClientId = -1)
    {
        if(!type.Contains(" - Placed")) type += " - Placed";
        string uuid = BrickId.FetchNewBrickID();
        GameObject brickObject;
        if (headClientId == -1)
        {
            brickObject = GameObject.Instantiate(BrickPrefabCache.GetInstance().Get(type), pos, rot);
        }
        else
        {
            AvatarManager avatarManager = AvatarManager.GetInstance();
            Transform headTransform = avatarManager.localAvatar.head.transform;

            brickObject = GameObject.Instantiate(BrickPrefabCache.GetInstance().Get(type), headTransform);
            brickObject.transform.localPosition = pos;
            brickObject.transform.localRotation = rot;
        }
        BrickAttach newBrickAttach = brickObject.GetComponent<BrickAttach>();

        newBrickAttach.Color = ColorInt.IntToColor32(color);

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

        BrickDestroyer destroyer = BrickDestroyer.GetInstance();

        if (session.isSinglePlayer)
            destroyer.DelayedDestroy(gameObject);
    }
}
