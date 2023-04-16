using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

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
        GameObject brick = GameObject.Instantiate(
            Resources.Load<GameObject>(brickPrefabName),
            transform.position,
            transform.rotation
        );

        brick.transform.Rotate(_brickRotation, Space.Self);

        BrickAttach attach = brick.GetComponent<BrickAttach>();
        attach.Color = BrickColorMap.ColorFromID(materialId);
        brick.GetComponent<BrickUuid>().uuid = BrickId.FetchNewBrickID();

        return brick;
    }
}
