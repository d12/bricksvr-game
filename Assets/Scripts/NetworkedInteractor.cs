using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Normal.Realtime;


public class NetworkedInteractor : MonoBehaviour
{
    private XRBaseInteractor _interactor;
    // Start is called before the first frame update
    private void Awake()
    {
        _interactor = GetComponent<XRBaseInteractor>();
    }

    private void OnEnable()
    {
        _interactor.onSelectEnter.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        _interactor.onSelectEnter.RemoveListener(OnGrab);
    }

    private void OnGrab(XRBaseInteractable interactable)
    {
        RealtimeTransform rt = interactable.GetComponent<RealtimeTransform>();
        if (rt != null)
        {
            rt.RequestOwnership();
            if (!rt.isOwnedLocallySelf)
            {
                Debug.Log("Attempted to pick up object but didn't obtain ownership. Dropping for 0.5s");
                interactable.interactionLayerMask = 0;
                StartCoroutine(DelayedReEnableInteractable(interactable));
            }
        }
    }

    private static IEnumerator DelayedReEnableInteractable(XRBaseInteractable interactable)
    {
        yield return new WaitForSeconds(0.5f);
        interactable.interactionLayerMask = ~0;
    }
}