using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BrickRegrabFix : QuickInteractable
{
    private XRGrabInteractable _grabInteractable;
    private XRInteractionManager _xrInteractionManager;
    private BrickAttachDetector _brickAttachDetector;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _xrInteractionManager = _grabInteractable.interactionManager;
        _brickAttachDetector = GetComponent<BrickAttachDetector>();
    }

    public override void Interact(QuickInteractor interactor)
    {
        XRDirectInteractor xrInteractor = interactor.GetComponent<XRDirectInteractor>();
        if (!xrInteractor) return;

        if (_grabInteractable.isSelected && (_grabInteractable.m_SelectingInteractor != xrInteractor))
        {
            _brickAttachDetector.skipGrabCallbacks = true;

            _grabInteractable.m_SelectingInteractor.GetComponent<TemporarilyDisableHand>().TemporarilyDisable();
            _xrInteractionManager.ForceSwap(_grabInteractable.m_SelectingInteractor, xrInteractor, _grabInteractable);


            _brickAttachDetector.skipGrabCallbacks = false;
        }
    }
}
