using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UpdateConnectorLayerWhenHeld : MonoBehaviour
{
    public GameObject maleConnectorParent;
    public GameObject femaleConnectorParent;

    public GameObject[] connectors;

    // Start is called before the first frame update
    private void Awake()
    {
        XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
        if (interactable == null) return;
        interactable.onSelectEnter.AddListener(OnGrab);
        interactable.onSelectExit.AddListener(OnRelease);
    }

    private void OnGrab(XRBaseInteractor interactor)
    {
        foreach (GameObject connector in connectors)
        {
            connector.layer = 19; // held lego connector
        }
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        foreach (GameObject connector in connectors)
        {
            connector.layer = 15; // connector
        }
    }

    private void OnValidate()
    {
        List<GameObject> connectorsList = new List<GameObject>();
        foreach (Transform child in maleConnectorParent.transform)
        {
            connectorsList.Add(child.gameObject);
        }

        foreach (Transform child in femaleConnectorParent.transform)
        {
            connectorsList.Add(child.gameObject);
        }
        connectors = connectorsList.ToArray();
    }
}
