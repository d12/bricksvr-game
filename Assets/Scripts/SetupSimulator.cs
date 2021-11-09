using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SetupSimulator : MonoBehaviour
{
    [FormerlySerializedAs("VRController")] public GameObject vrController;

    private void Start()
    {
        if (Application.isEditor)
        {
            vrController.GetComponent<VRSimulator>().enabled = true;
        }
    }
}
