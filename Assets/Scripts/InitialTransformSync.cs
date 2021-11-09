using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

public class InitialTransformSync : RealtimeComponent<InitialTransformModel>
{
    private NormalSessionManager _normalSessionManager;
    private UserSettings _userSettings;

    private void Awake()
    {
        _normalSessionManager = GameObject.FindWithTag("NormalSessionManager").GetComponent<NormalSessionManager>();
        _userSettings = UserSettings.GetInstance();
    }
    private void PositionSet()
    {
        transform.position = model.position;
        if(!_normalSessionManager.Loading() && model.position != Vector3.zero && _userSettings.BrickClickSoundsEnabled)
            BrickSounds.GetInstance().PlayBrickSnapSound(model.position);
    }

    private void RotationSet()
    {
        transform.rotation = model.rotation;
    }

    protected override void OnRealtimeModelReplaced(InitialTransformModel previousModel, InitialTransformModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.positionDidChange -= PositionDidChange;
            previousModel.rotationDidChange -= RotationDidChange;
        }

        if (currentModel != null)
        {
            PositionSet();
            RotationSet();

            currentModel.positionDidChange += PositionDidChange;
            currentModel.rotationDidChange += RotationDidChange;
        }
    }

    private void PositionDidChange(InitialTransformModel model, Vector3 value)
    {
        PositionSet();
    }

    private void RotationDidChange(InitialTransformModel model, Quaternion value)
    {
        RotationSet();
    }

    public void SetPosition(Vector3 pos)
    {
        model.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        model.rotation = rot;
    }
}