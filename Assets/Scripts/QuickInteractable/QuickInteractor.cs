using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class QuickInteractor : MonoBehaviour
{
    public bool leftHand;
    public LayerMask interactionMask;

    private Dictionary<GameObject, QuickInteractable> _hoveredObjects;

    private OVRInput.Button _trigger;

    private GameObject _tempHoveredObject;
    private QuickInteractable _tempHoveredInteractable;
    private BrickHover _brickHover;

    private bool _debugGrabEnabled;

    private bool _interacting;

    void Awake()
    {
        _hoveredObjects = new Dictionary<GameObject, QuickInteractable>();
        _trigger = leftHand ? OVRInput.Button.PrimaryHandTrigger : OVRInput.Button.SecondaryHandTrigger;
        _brickHover = GetComponent<BrickHover>();
        _debugGrabEnabled = leftHand && Application.isEditor;
    }

    private void OnTriggerEnter(Collider c)
    {
        GameObject obj = c.gameObject;

        if (Contains(interactionMask, obj.layer) )
        {
            QuickInteractable interactable = obj.GetComponentInParent<QuickInteractable>();
            if (interactable != null && !_hoveredObjects.ContainsKey(interactable.gameObject))
            {
                _hoveredObjects[interactable.gameObject] = interactable;
                _brickHover.AddToHoverList(interactable.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider c)
    {
        QuickInteractable interactable = c.gameObject.GetComponentInParent<QuickInteractable>();
        if (interactable == null) return;

        _hoveredObjects.Remove(interactable.gameObject);
        _brickHover.RemoveFromHoverList(interactable.gameObject);
    }
    void Update()
    {
        if (_interacting)
        {
            if ((!_debugGrabEnabled && !OVRInput.Get(_trigger, OVRInput.Controller.Touch)) || (_debugGrabEnabled && !Input.GetMouseButton(0)))
            {
                _interacting = false;
            }
        }
        else
        {
            if (_hoveredObjects.Count == 0) return;
            if (!OVRInput.Get(_trigger, OVRInput.Controller.Touch) && !(_debugGrabEnabled && Input.GetMouseButton(0))) return;

            CleanHoveredList();
            if (_hoveredObjects.Count == 0) return;

            _tempHoveredInteractable = _hoveredObjects.Count == 1
                ? _hoveredObjects.First().Value
                : _hoveredObjects.OrderBy(o => (o.Key.transform.position - transform.position).sqrMagnitude).First().Value;

            if (_tempHoveredInteractable == null) return;
            if (_tempHoveredInteractable.gameObject != _brickHover.HoveredBrick() && _tempHoveredInteractable.GetComponent<BrickPickerBrick>() == null) return;

            _tempHoveredInteractable.Interact(this);
            _interacting = true;
        }
    }

    private void CleanHoveredList()
    {
        _hoveredObjects
            .Keys
            .Where(k => k == null || !k.activeInHierarchy)
            .ToList()
            .ForEach(k => _hoveredObjects.Remove(k));
    }

    public Dictionary<GameObject, QuickInteractable> HoveredObjects()
    {
        return _hoveredObjects;
    }

    private static bool Contains(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}
