using UnityEngine.Serialization;
using UnityEngine;

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
