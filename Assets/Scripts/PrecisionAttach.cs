using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// The Unity XR interaction framework does not support precision grab out of the box
// That's so that when you pick something up it doesn't snap immediately to the center of your hand.
public class PrecisionAttach : MonoBehaviour
{
    private Transform _mAttachPoint;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    // Start is called before the first frame update
    private void Awake()
    {
        _mAttachPoint = GetComponent<XRBaseInteractor>().attachTransform;
    }

    private void OnEnable()
    {
        GetComponent<XRBaseInteractor>().onSelectEnter.AddListener(UpdatePos);
        GetComponent<XRBaseInteractor>().onSelectExit.AddListener(ResetPos);
    }

    private void OnDisable()
    {
        GetComponent<XRBaseInteractor>().onSelectEnter.RemoveListener(UpdatePos);
        GetComponent<XRBaseInteractor>().onSelectExit.RemoveListener(ResetPos);
    }

    public void UpdatePos(XRBaseInteractable interactable)
    {
        _originalPosition = _mAttachPoint.localPosition;
        _originalRotation = _mAttachPoint.localRotation;

        _mAttachPoint.position = interactable.transform.position;
        _mAttachPoint.rotation = interactable.transform.rotation;
    }

    private void ResetPos(XRBaseInteractable interactable)
    {
        _mAttachPoint.localPosition = _originalPosition;
        _mAttachPoint.localRotation = _originalRotation;
    }
}