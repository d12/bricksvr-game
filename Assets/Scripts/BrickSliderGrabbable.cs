using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BrickSliderGrabbable : MonoBehaviour
{
    public SelectedCategoryManager selectedCategoryManager;
    public Material hoveredMaterial;
    private Material _unhoveredMaterial;
    public Renderer sliderRenderer;
    public bool beingGrabbed;
    private bool _beingGrabbedWithLeftHand;
    private Material[] _materials;

    private void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        _unhoveredMaterial = sliderRenderer.materials[1];
        grabInteractable.onHoverEnter.AddListener(OnSliderHovered);
        grabInteractable.onHoverExit.AddListener(OnSliderUnhovered);
        grabInteractable.onSelectEnter.AddListener(OnSliderGrabbed);
        grabInteractable.onSelectExit.AddListener(OnSliderReleased);
    }

    private void Update()
    {
        if(beingGrabbed)
            selectedCategoryManager.SetPosition(transform.position, _beingGrabbedWithLeftHand);
    }

    private void LateUpdate()
    {
        transform.position = selectedCategoryManager.transform.position;
    }

    private void SetHoveredMaterial()
    {
        _materials = sliderRenderer.materials;
        _materials[1] = hoveredMaterial;
        sliderRenderer.materials = _materials;
    }

    private void SetUnhoveredMaterial()
    {
        _materials = sliderRenderer.materials;
        _materials[1] = _unhoveredMaterial;
        sliderRenderer.materials = _materials;
    }

    private void OnSliderHovered(XRBaseInteractor interactor)
    {
        if(!beingGrabbed)
            SetHoveredMaterial();
    }

    private void OnSliderUnhovered(XRBaseInteractor interactor)
    {
        SetUnhoveredMaterial();
    }

    private void OnSliderGrabbed(XRBaseInteractor interactor)
    {
        beingGrabbed = true;
        _beingGrabbedWithLeftHand = interactor.GetComponent<QuickInteractor>().leftHand;

        selectedCategoryManager.PlayerGrabbed();
        SetUnhoveredMaterial();
    }

    private void OnSliderReleased(XRBaseInteractor interactor)
    {
        beingGrabbed = false;
        selectedCategoryManager.PlayerReleased();
    }
}
