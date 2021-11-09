using System;
using System.Collections;
using System.Collections.Generic;
using Sigtrap.VrTunnellingPro;
using UnityEngine;

public class MovementVignette : MonoBehaviour
{
    public Tunnelling tunnelling;
    private bool _vignetteEnabled;

    private float _originalVelocityMin;
    private float _originalVelocityMax;

    private void Start()
    {
        _vignetteEnabled = false;
        tunnelling.enabled = _vignetteEnabled;
        _originalVelocityMin = tunnelling.velocityMin;
        _originalVelocityMax = tunnelling.velocityMax;
    }

    public void VignetteStrengthUpdated(float strength)
    {
        if (strength == 0)
        {
            tunnelling.angularVelocityStrength = 0;
            tunnelling.velocityStrength = 0;
        }
        else
        {
            tunnelling.angularVelocityStrength = strength + 0.4f;
            tunnelling.velocityStrength = strength + 0.6f;
        }
    }

    public void WithVignetteDisabled(Action action)
    {
        tunnelling.enabled = false;

        action.Invoke();

        tunnelling.enabled = _vignetteEnabled;
    }

    public void SetPlayerScale(float scale)
    {
        tunnelling.velocityMin = _originalVelocityMin * scale;
        tunnelling.velocityMax = _originalVelocityMax * scale;
    }
}
