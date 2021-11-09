using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class ForceHandPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        Hand handScript = c.GetComponentInParent<Hand>();
        if (handScript == null)
            return;

        handScript.SetPointLock(true);
    }

    private void OnTriggerExit(Collider c)
    {
        Hand handScript = c.GetComponentInParent<Hand>();
        if (handScript == null)
            return;

        handScript.SetPointLock(false);
    }
}
