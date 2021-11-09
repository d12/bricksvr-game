using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RotationSettingHandler : MonoBehaviour
{
    private SnapTurnProvider _snapTurn;
    private SmoothTurn _smoothTurn;
    private void Awake()
    {
        _snapTurn = GetComponent<SnapTurnProvider>();
        _smoothTurn = GetComponent<SmoothTurn>();
    }

    public void OnRotationSettingUpdated(bool smoothRotationEnabled)
    {
        _smoothTurn.enabled = smoothRotationEnabled;
        _snapTurn.enabled = !smoothRotationEnabled;
    }

    public void OnRotationSpeedUpdated(float rotationSpeed)
    {
        _snapTurn.debounceTime = (1 - rotationSpeed) * 0.5f + 0.05f;
        _smoothTurn.turnSpeed = -0.454545f + (60.9090f * rotationSpeed) + (90.909f * rotationSpeed * rotationSpeed);
    }
}