using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractablePlacedBrick : QuickInteractable
{
    public override void Interact(QuickInteractor interactor)
    {
        GameObject brick = BrickSwapper.SwapToRealBrick(gameObject);
        // brick.GetComponent<BoxCollider>().enabled = false;
        // XRDirectInteractor xrInteractor = interactor.GetComponent<XRDirectInteractor>();
        // xrInteractor.interactionManager.ForceSelect(xrInteractor, brick.GetComponent<XRGrabInteractable>());
    }
}
