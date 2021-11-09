using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class BrickPickerBrick : QuickInteractable
{
    public Image backgroundImage;
    [FormerlySerializedAs("text")] public TextMeshProUGUI brickLabel;

    private BrickData.Brick _brickData;
    private MeshRenderer _brickMeshRenderer;
    private static readonly int ShaderColorProperty = Shader.PropertyToID("_Color");

    public Color32 backgroundColor = new Color32(229, 229, 229, 20);
    public Color32 hoveredColor = new Color32(170, 170, 170, 60);
    private Color _color;
    private MaterialPropertyBlock _materialPropertyBlock;
    private BrickPickerManager _manager;
    private Transform _transform;
    private bool _isHovered;
    private Collider _hoveringCollider;
    private float _lastHoveredFixedTime;

    public void Initialize(BrickData.Brick brickData, Color color, BrickPickerManager manager)
    {
        MeshFilter mf = GetComponentInChildren<MeshFilter>();
        _brickMeshRenderer = GetComponentInChildren<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _manager = manager;
        _transform = transform;

        _brickData = brickData;
        brickLabel.text = brickData.DisplayName;
        mf.mesh = brickData.Mesh;
        mf.transform.localEulerAngles += _brickData.Rotation;
        mf.transform.localScale *= _brickData.UIScaleModifier;
        SetColor(color);
    }

    private void Update()
    {
        if(_isHovered)
            _manager.AddHoveredTile(this, Vector3.Distance(_transform.position, _hoveringCollider.transform.position));
    }

    private void FixedUpdate()
    {
        _isHovered = Math.Abs(Time.fixedTime - _lastHoveredFixedTime) < 0.02f;
    }

    public void SetColor(Color color)
    {
        _color = color;
        _materialPropertyBlock.SetColor(ShaderColorProperty, color);

        _brickMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public override void Interact(QuickInteractor interactor)
    {
        if (interactor.GetComponent<XRDirectInteractor>().selectTarget != null)
            return;

        if (!_manager.IsMenuFullyOpen)
            return;

        GameObject brick = Realtime.Instantiate(
            _brickData.PrefabName,
            interactor.transform.position,
            _brickMeshRenderer.transform.rotation,
            ownedByClient: false,
            preventOwnershipTakeover: false,
            destroyWhenOwnerOrLastClientLeaves: true,
            useInstance: null
        );

        brick.GetComponent<Rigidbody>().isKinematic = false;

        brick.GetComponent<RealtimeView>().ClearOwnership();
        BuildingBrickSync sync = brick.GetComponent<BuildingBrickSync>();
        sync.EnableNewColors();
        sync.SetColor(ColorInt.ColorToInt(_color));
        sync.SetUuid(BrickId.FetchNewBrickID());

        FadeObjectScaleOnSpawn fadeComponent = brick.AddComponent<FadeObjectScaleOnSpawn>();
        fadeComponent.objectToScale = brick.transform.Find("CombinedModel").gameObject;
    }

    // Because we make the hand collider smaller when the user closes their fist, OnTriggerExit sometimes doesn't get called.
    // So we need to use OnTriggerStay instead. The order of execution goes FixedUpdate, OnTriggerStay, Update, so FixedUpdate
    // clear the bool every physics frame and then this function will set it before Update() if a hand is intersecting the collider.
    private void OnTriggerStay(Collider c)
    {
        _hoveringCollider = c;
        _lastHoveredFixedTime = Time.fixedTime;
    }

    private void OnTriggerExit(Collider c)
    {
        _isHovered = false;
        SetIsHovered(false);
    }

    public void SetIsHovered(bool hovered)
    {
        backgroundImage.color = hovered ? hoveredColor : backgroundColor;
    }

    public Color Color()
    {
        return _color;
    }

    public string PrefabName()
    {
        return _brickData.PrefabName;
    }
}
