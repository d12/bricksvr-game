using System.Collections;
using System.Collections.Generic;
using System.Security;
using Normal.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableSpawner : QuickInteractable
{
    public XRInteractionManager interactionManager;
    public string brickPrefabName;
    public int materialId;

    public Material inGameMaterial;

    public GameObject model;
    private static readonly int Color = Shader.PropertyToID("_Color");

    private Vector3 _brickRotation;

    private void Start()
    {
        GetComponentInChildren<MeshRenderer>().material = inGameMaterial;
        Color32 spawnerColor = BrickColorMap.ColorFromID(materialId);
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor(Color, spawnerColor);

        MeshRenderer renderer = model.GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(props);

        _brickRotation = BrickData.BrickByPrefabName(brickPrefabName).Rotation;
    }

    public override void Interact(QuickInteractor interactor)
    {
        GameObject brick = CreateBrick();
    }

    public GameObject CreateBrick()
    {
        GameObject brick = Realtime.Instantiate(
            brickPrefabName,
            transform.position,
            transform.rotation,
            ownedByClient: false,
            preventOwnershipTakeover: false,
            destroyWhenOwnerOrLastClientLeaves: true,
            useInstance: null
        );

        brick.transform.Rotate(_brickRotation, Space.Self);

        brick.GetComponent<RealtimeView>().ClearOwnership();
        BuildingBrickSync sync = brick.GetComponent<BuildingBrickSync>();
        sync.EnableNewColors();
        sync.SetColor(ColorInt.ColorToInt(BrickColorMap.ColorFromID(materialId)));
        sync.SetUuid(BrickId.FetchNewBrickID());

        return brick;
    }
}
