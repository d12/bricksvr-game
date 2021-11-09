using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    private static HapticsManager _instance;

    public static HapticsManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindGameObjectWithTag("HapticsManager").GetComponent<HapticsManager>();
        }

        return _instance;
    }
    public void PlayHaptics(float frequency, float amplitude, float duration, bool rightHand, bool leftHand)
    {
        StartCoroutine(PlayHapticsIEnum(frequency, amplitude, duration, rightHand, leftHand));
    }

    // Use when you don't want to auto-disable haptics.
    public void StartHaptics(float frequency, float amplitude, bool rightHand, bool leftHand)
    {
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);

        Debug.Log($"Playing @ {amplitude},{frequency}");
    }

    public void EndHaptics(bool rightHand, bool leftHand)
    {
        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);

        Debug.Log($"Stopping");
    }

    public static IEnumerator PlayHapticsIEnum(float frequency, float amplitude, float duration, bool rightHand, bool leftHand)
    {
        if (rightHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);

        yield return new WaitForSeconds(duration);

        if (rightHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        if (leftHand) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
}
