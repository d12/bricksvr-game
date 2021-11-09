using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnableTeleportRayOnSelect : MonoBehaviour
{
    private XRRayInteractor _interactor;

    private XRInteractorLineVisual _lineVisual;

    private void OnEnable()
    {
        _interactor = GetComponent<XRRayInteractor>();
        _lineVisual = GetComponent<XRInteractorLineVisual>();
        _interactor.onSelectEnter.AddListener(OnEnter);
        _interactor.onSelectExit.AddListener(OnExit);
    }

    private void OnDisable()
    {
        _interactor.onSelectEnter.RemoveListener(OnEnter);
        _interactor.onSelectExit.RemoveListener(OnExit);
    }

    private void OnEnter(XRBaseInteractable _)
    {
        _lineVisual.enabled = true;
    }

    private void OnExit(XRBaseInteractable _)
    {
        _lineVisual.enabled = false;

        GameObject[] reticles = GameObject.FindGameObjectsWithTag("TeleportReticle");
        foreach(GameObject reticle in reticles)
        {
            Vector3 pos = reticle.transform.position;
            reticle.transform.position = new Vector3(pos.x, pos.y - 10, pos.z);
        }
    }
}
