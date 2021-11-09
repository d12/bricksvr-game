using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TemporarilyDisableHand : MonoBehaviour
{
    private XRDirectInteractor _xrDirectInteractor;
    private LayerMask _previousLayerMask;
    private Coroutine _coroutine;

    // Start is called before the first frame update
    void Start()
    {
        _xrDirectInteractor = GetComponent<XRDirectInteractor>();
    }

    public void TemporarilyDisable()
    {
        if (_coroutine == null)
            _coroutine = StartCoroutine(TemporarilyDisableIEnum(0.4f));
    }

    private IEnumerator TemporarilyDisableIEnum(float time)
    {
        _previousLayerMask = _xrDirectInteractor.InteractionLayerMask;
        _xrDirectInteractor.InteractionLayerMask = 0;

        yield return new WaitForSeconds(time);

        if (_xrDirectInteractor.InteractionLayerMask == 0)
        {
            _xrDirectInteractor.InteractionLayerMask = _previousLayerMask;
        }

        _coroutine = null;
    }
}
