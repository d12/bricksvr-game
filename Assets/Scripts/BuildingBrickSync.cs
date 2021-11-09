using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

public class BuildingBrickSync : RealtimeComponent<BuildingBrickModel>
{
    private void BrickAttachedSet()
    {
        if (model.attached)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void MatIdSet()
    {
        if(!model.usingNewColors)
            GetComponent<BrickAttach>().Color = BrickColorMap.ColorFromID(model.matId);
    }

    private void ColorSet()
    {
        if (model.usingNewColors)
            GetComponent<BrickAttach>().Color = ColorInt.IntToColor32(model.color);
    }

    private void UuidSet()
    {
        if (string.IsNullOrEmpty(model.uuid)) return;

        BrickStore brickStore = BrickStore.GetInstance();

        GameObject existingBrick = brickStore.Get(model.uuid);
        if (existingBrick) existingBrick.GetComponent<BrickAttach>().DelayedDestroy();

        brickStore.Put(model.uuid, gameObject);

        BrickUuid brickUuid = GetComponent<BrickUuid>();
        if (brickUuid.uuid != null)
            brickStore.Delete(brickUuid.uuid); // If another client sets the value of this UUID, we should remove our old uuid from the store.

        GetComponent<BrickUuid>().uuid = model.uuid;
    }

    protected override void OnRealtimeModelReplaced(BuildingBrickModel previousModel, BuildingBrickModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.attachedDidChange -= AttachedDidChange;
            previousModel.uuidDidChange -= UuidDidChange;
            previousModel.matIdDidChange -= MatIdDidChange;
            previousModel.colorDidChange -= ColorDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.attached = false;
                currentModel.bricksAbove = "";
                currentModel.bricksBelow = "";
            }

            UuidSet();
            MatIdSet();
            ColorSet();

            currentModel.attachedDidChange += AttachedDidChange;
            currentModel.uuidDidChange += UuidDidChange;
            currentModel.matIdDidChange += MatIdDidChange;
            currentModel.colorDidChange += ColorDidChange;
        }
    }

    private void AttachedDidChange(BuildingBrickModel model, bool value)
    {
        BrickAttachedSet();
    }

    private void MatIdDidChange(BuildingBrickModel model, int matId)
    {
        MatIdSet();
    }

    private void ColorDidChange(BuildingBrickModel model, int color)
    {
        ColorSet();
    }

    private void UuidDidChange(BuildingBrickModel model, string uuid)
    {
        UuidSet();
    }

    public void SetAttached(bool attached)
    {
        if (model != null)
        {
            model.attached = attached;
        }
        else
        {
            Debug.Log("WHY IS MODEL NULL THAT SHOULDN'T HAPPEN");
        }
    }

    public void EnableNewColors()
    {
        if (model != null)
        {
            model.usingNewColors = true;
        }
        else
        {
            Debug.Log("WHY IS MODEL NULL");
        }
    }

    public void SetColor(int color)
    {
        if (model != null)
        {
            model.color = color;
        }
        else
        {
            Debug.Log("WHY IS MODEL NULL");
        }
    }

    public void SetUuid(string uuid)
    {
        model.uuid = uuid;
    }
}
