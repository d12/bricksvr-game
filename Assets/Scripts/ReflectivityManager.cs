using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectivityManager : MonoBehaviour
{
    private static readonly float[] ReflectivityValues =
    {
        0f,
        0.25f,
        0.5f,
        0.75f,
        0.9f,
        1.25f,
        1.5f
    };

    public float reflectivityMultiplier = 1f;

    public static float GetReflectivityFromSliderValue(int sliderValue)
    {
        return ReflectivityValues[sliderValue - 1];
    }

    public void SetReflectivity(int sliderValue)
    {
        float reflectivity = GetReflectivityFromSliderValue(sliderValue);

        RenderSettings.reflectionIntensity = reflectivity;
    }
}
