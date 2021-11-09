using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomOwnershipSync : RealtimeComponent<RoomOwnershipModel>
{
    public QuickInteractor leftHandInteractor;
    public QuickInteractor rightHandInteractor;

    public KeepPlayerOutOfWalls keepPlayerOutOfWalls;

    private LayerMask _interactorUnlockedMask;
    private LayerMask _interactorLockedMask;

    public Toggle roomLockedToggle;
    public Toggle lowGravityToggle;
    [FormerlySerializedAs("allowedToMoveThroughBricksToggle")] public Toggle blockedFromMovingThroughBricksToggle;

    public bool overrideLocked;
    public string overrideOwnerId;

    private string _ownerIdPrefix;

    private static Vector3 _lowGravityVector = new Vector3(0, -1f, 0);
    private static Vector3 _regularGravityVector = new Vector3(0, -9.8f, 0);

    public string roomName;

    private void Awake()
    {
        _interactorUnlockedMask = LayerMask.GetMask("placed lego", "spawner", "lego");
        _interactorLockedMask = LayerMask.GetMask("spawner", "lego");
    }

    public bool Locked()
    {
        return model.locked;
    }

    public bool LowGravity()
    {
        return model.lowGravity;
    }

    public bool BlockedFromMovingThroughBricks()
    {
        return model.blockedFromMovingThroughBricks;
    }

    private string OwnerIdPrefix()
    {
        return _ownerIdPrefix;
    }

    public bool IsRoomOwner()
    {
        return OwnerIdPrefix() == UserId.GetShortId();
    }

    public bool IsRoomOwner(string shortId)
    {
        return OwnerIdPrefix() == shortId;
    }

    public bool IsUserAllowedToMakeChanges()
    {
        return !Locked() || (OwnerIdPrefix() == UserId.GetShortId());
    }

    private void LockedSet()
    {
        if (IsUserAllowedToMakeChanges())
        {
            leftHandInteractor.interactionMask = _interactorUnlockedMask;
            rightHandInteractor.interactionMask = _interactorUnlockedMask;
        }
        else
        {
            leftHandInteractor.interactionMask = _interactorLockedMask;
            rightHandInteractor.interactionMask = _interactorLockedMask;
        }

        roomLockedToggle.SetIsOnWithoutNotify(model.locked);
    }

    private void LowGravitySet()
    {
        lowGravityToggle.SetIsOnWithoutNotify(model.lowGravity);

        Physics.gravity = model.lowGravity ? _lowGravityVector : _regularGravityVector;
    }

    private void BlockedFromMovingThroughBricksSet()
    {
        blockedFromMovingThroughBricksToggle.SetIsOnWithoutNotify(model.blockedFromMovingThroughBricks);
        keepPlayerOutOfWalls.SetIsAllowedToGoThroughBricks(!model.blockedFromMovingThroughBricks);
    }

    protected override void OnRealtimeModelReplaced(RoomOwnershipModel previousModel, RoomOwnershipModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.lockedDidChange -= LockedDidChange;
            previousModel.lowGravityDidChange -= LowGravityDidChange;
            previousModel.blockedFromMovingThroughBricksDidChange -= BlockedFromMovingThroughBricksDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.locked = false;
                currentModel.lowGravity = false;
                currentModel.blockedFromMovingThroughBricks = false;
            }
            LockedSet();
            LowGravitySet();
            BlockedFromMovingThroughBricksSet();

            currentModel.lockedDidChange += LockedDidChange;
            currentModel.lowGravityDidChange += LowGravityDidChange;
            currentModel.blockedFromMovingThroughBricksDidChange += BlockedFromMovingThroughBricksDidChange;
        }
    }

    private void LockedDidChange(RoomOwnershipModel model, bool locked)
    {
        LockedSet();
    }

    private void LowGravityDidChange(RoomOwnershipModel model, bool lowGravity)
    {
        LowGravitySet();
    }

    private void BlockedFromMovingThroughBricksDidChange(RoomOwnershipModel model, bool blockedFromMovingThroughBricks)
    {
        BlockedFromMovingThroughBricksSet();
    }

    public void SetLocked(bool locked)
    {
        if (locked || UserId.GetShortId() == _ownerIdPrefix)
        {
            model.locked = locked;
            if(UserId.GetShortId() == _ownerIdPrefix) BrickServerInterface.GetInstance().SetLocked(locked, realtime);
        }
        else
        {
            Debug.LogError("Rejecting lock change, permission error");
        }
    }

    public void SetGravity(bool lowGravity)
    {
        if (UserId.GetShortId() == _ownerIdPrefix)
        {
            model.lowGravity = lowGravity;
        }
        else
        {
            Debug.LogError("Rejecting lock change, permission error");
        }
    }

    public void SetBlockedFromMovingThroughBricks(bool blocked)
    {
        if (UserId.GetShortId() == _ownerIdPrefix)
        {
            model.blockedFromMovingThroughBricks = blocked;
        }
        else
        {
            Debug.LogError("Rejecting lock change, permission error");
        }
    }

    public void SetOwnerIdPrefix(string ownerIdPrefix)
    {
        _ownerIdPrefix = ownerIdPrefix;
    }
}
