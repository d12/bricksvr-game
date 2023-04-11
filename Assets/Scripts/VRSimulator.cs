using UnityEngine;
using OVR.OpenVR;

public class VRSimulator : MonoBehaviour
{
    void Start() {
        if(OVRManager.loadedXRDevice == 0)
            Destroy(this);

        transform.Find("Camera Offset/Head").localPosition += new Vector3(0, 1.43f, 0);
    }

    // Inspired Source: https://github.com/ezefranca/NFCPlay/blob/master/_jogo/VRSJ/Assets/Codes/VRSimulator.cs
    void Update()
    {
        if(OVRManager.loadedXRDevice == 0)
            return;
        
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 neckmov = new Vector3(-Input.GetAxis("Mouse Y"), 0, 0);
        transform.Rotate(neckmov);

        Vector3 bodymov = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.parent.Rotate(bodymov);
    }
}