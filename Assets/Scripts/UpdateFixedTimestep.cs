using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpdateFixedTimestep : MonoBehaviour
{
    private void Start()
    {
        // return;
        float frameRate = 90f;
        OVRPlugin.SystemHeadset headsetType = OVRPlugin.GetSystemHeadsetType();

        switch (headsetType)
        {
            case OVRPlugin.SystemHeadset.Oculus_Quest:
                frameRate = 72f;
                break;

            case OVRPlugin.SystemHeadset.Rift_S:
                frameRate = 80f;
                break;

            case OVRPlugin.SystemHeadset.Oculus_Link_Quest:
                frameRate = 90f;
                break;

            case OVRPlugin.SystemHeadset.Rift_CV1:
                frameRate = 90f;
                break;

            case OVRPlugin.SystemHeadset.Rift_DK1:
                frameRate = 60f;
                break;

            case OVRPlugin.SystemHeadset.Rift_DK2:
                frameRate = 75f;
                break;
        }

        Time.fixedDeltaTime = 1 / frameRate;

        Debug.Log($"Device is {headsetType}, framerate is {frameRate}");
    }
}
