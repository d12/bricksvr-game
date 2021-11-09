using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDropWhenNotOwned : MonoBehaviour
{
    private XRGrabInteractable _interactable;
    private XRBaseInteractor _interactor;

    private RealtimeTransform _realtimeTransform;
    // Start is called before the first frame update

    private bool _held;

    private void Awake()
    {
        _interactable = GetComponent<XRGrabInteractable>();
        _realtimeTransform = GetComponent<RealtimeTransform>();

        if (_interactable == null || _realtimeTransform == null) return;

        _interactable.onSelectEnter.AddListener(ObjectGrabbed);
        _interactable.onSelectExit.AddListener(ObjectDropped);
    }

    private void ObjectGrabbed(XRBaseInteractor interactor)
    {
        _interactor = interactor;
        _held = true;
    }

    private void ObjectDropped(XRBaseInteractor interactor)
    {
        _interactor = null;
        _held = false;
    }

    private void Update()
    {
        if (!_held) return;

        if (_realtimeTransform.isOwnedLocallySelf) return;

        _interactable.interactionLayerMask = 0;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        StartCoroutine(DelayedReEnableInteractable(_interactable));
        ObjectDropped(_interactor);
    }

    private static IEnumerator DelayedReEnableInteractable(XRBaseInteractable interactable)
    {
        yield return new WaitForSeconds(2f);
        interactable.interactionLayerMask = ~0;
    }
}
