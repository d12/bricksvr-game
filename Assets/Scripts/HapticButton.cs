using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class HapticButton : MonoBehaviour
{
    private HapticsManager _hapticsManager;
    private SphereCollider _collider;
    private Transform _transform;

    public float amountToPushToActivate;
    private float _outerRadius;
    private float InnerRadius => (1f - amountToPushToActivate) * _outerRadius;

    private bool _isPressed;
    private Coroutine _pressActivationHapticsCoroutine;

    private readonly Dictionary<Collider, Hand> _colliderToHandCache = new Dictionary<Collider, Hand>();

    private void Awake()
    {
        _transform = transform;
        _hapticsManager = HapticsManager.GetInstance();
        _collider = GetComponent<SphereCollider>();
        _outerRadius = _collider.radius * _collider.transform.lossyScale.magnitude;
        _collider.isTrigger = true;
    }

    private void OnTriggerStay(Collider c)
    {
        if (_isPressed || _pressActivationHapticsCoroutine != null)
            return;

        Hand hand = GetHandForCollider(c);
        if (hand == null) return;

        Vector3 ourCenterPoint = _transform.position;
        float distanceBetweenColliders = Mathf.Abs((c.transform.position - ourCenterPoint).magnitude);
        float ratioInside = 1f - (distanceBetweenColliders / _outerRadius);

        // Colliders can intersect but the center of the finger is still outside of the collider
        if (ratioInside < 0f)
        {
            _hapticsManager.EndHaptics(!hand.leftHand, hand.leftHand);
            return;
        }

        if (ratioInside > amountToPushToActivate)
        {
            _isPressed = true;
            Debug.Log("ACTIVATION!");
            _pressActivationHapticsCoroutine = StartCoroutine(RunActivationHaptics(hand.leftHand));
        }
        else
        {
            _hapticsManager.StartHaptics(ratioInside / 3f, ratioInside / 3f, !hand.leftHand, hand.leftHand);
        }
    }

    private void OnTriggerExit(Collider c)
    {
        _isPressed = false;

        if (_pressActivationHapticsCoroutine != null) return;

        Hand hand = GetHandForCollider(c);
        if (hand == null) return;

        _hapticsManager.EndHaptics(!hand.leftHand, hand.leftHand);
    }

    private Hand GetHandForCollider(Collider c)
    {
        if (!_colliderToHandCache.ContainsKey(c))
            _colliderToHandCache[c] = c.GetComponentInParent<Hand>();

        return _colliderToHandCache[c];
    }

    private IEnumerator RunActivationHaptics(bool leftHand)
    {
        yield return HapticsManager.PlayHapticsIEnum(1f, 1f, 0.05f, !leftHand, leftHand);
        _pressActivationHapticsCoroutine = null;
    }
}
