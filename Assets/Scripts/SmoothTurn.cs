using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SmoothTurn : MonoBehaviour
{
    public float turnSpeed = 1f;

    // Don't turn if the joystick is in the deadzone. Prevents drift from bad controllers and prevents accidental
    // rotation when pushing the joystick up/down
    public float deadZone = 0.15f;

    public float debugRotate = 0f;

    private XRRig rig;

    private void Start()
    {
        rig = GetComponent<XRRig>();
    }

    private void Update()
    {
        float rotationAmount = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch).x;
        if (Application.isEditor) rotationAmount = debugRotate;
        if (Mathf.Abs(rotationAmount) > deadZone)
        {
            Rotate(rotationAmount);
        }
    }

    private void Rotate(float amount)
    {
        rig.RotateAroundCameraUsingRigUp(amount * Time.deltaTime * turnSpeed);
    }
}
