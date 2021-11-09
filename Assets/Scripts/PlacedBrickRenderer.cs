using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedBrickRenderer : MonoBehaviour
{
    private PlacedBrickRendererManagerTwo placedBrickRendererManager;
    private BrickAttach _attach;
    public int instanceId;

    private bool _hasStartRun;
    private bool _hasSubmitted;
    private bool _usingHollowMesh;

    private void Start()
    {
        _hasStartRun = true;
        if(!_hasSubmitted)
            SubmitToRendererOnStart();
    }

    private void OnEnable()
    {
        _attach = GetComponent<BrickAttach>();
        placedBrickRendererManager = PlacedBrickRendererManagerTwo.GetInstance();

        if (_hasStartRun)
            SubmitToRendererOnStart();
    }

    private void SubmitToRendererOnStart()
    {
        if (_hasSubmitted)
            return;

        _hasSubmitted = true;

        placedBrickRendererManager.AddBrick(
            GetInstanceID(),
            transform,
            _attach.Color,
            _attach.hollowMesh);

        instanceId = GetInstanceID();

        foreach (LegoConnectorScript connectorScript in _attach.maleConnectorScripts)
        {
            placedBrickRendererManager.AddBrick(
                connectorScript.GetInstanceID(),
                connectorScript.transform,
                _attach.Color,
                _attach.studMesh);
        }
    }

    public void RerenderMeshes()
    {
        _hasSubmitted = true;

        RerenderBody();
        RerenderStuds();
    }

    private void RerenderBody()
    {
        placedBrickRendererManager.RemoveBrickSynchronously(GetInstanceID());
        placedBrickRendererManager.AddBrick(
            GetInstanceID(),
            transform,
            _attach.Color,
            _attach.renderHollowMesh ? _attach.hollowMesh : _attach.solidMesh);
    }

    private void RerenderStuds()
    {
        foreach (LegoConnectorScript connectorScript in _attach.maleConnectorScripts)
        {
            if (connectorScript.covered &&
                placedBrickRendererManager.HasObjectWithInstanceId(connectorScript.GetInstanceID()))
            {
                placedBrickRendererManager.RemoveBrick(connectorScript.GetHashCode());
            } else if (!connectorScript.covered &&
                       !placedBrickRendererManager.HasObjectWithInstanceId(connectorScript.GetInstanceID()))
            {
                placedBrickRendererManager.AddBrick(
                    connectorScript.GetInstanceID(),
                    connectorScript.transform,
                    _attach.Color,
                    _attach.studMesh);
            }
        }
    }

    private void OnDisable()
    {
        // If GetInstance() returns null, the app is shutting down and the PlacedBrickRendererManager has already been destroyed.
        PlacedBrickRendererManagerTwo.GetInstance()?.RemoveBrick(GetHashCode());

        foreach (LegoConnectorScript connectorScript in _attach.maleConnectorScripts)
        {
            PlacedBrickRendererManagerTwo.GetInstance()?.RemoveBrick(connectorScript.GetHashCode());
        }
    }
}