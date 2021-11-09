using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateBrickShininessSliderText : MonoBehaviour
{
    public TextMeshProUGUI sliderLabel;

    private void OnEnable()
    {
        SliderValueSet(UserSettings.GetInstance().BrickShininess);
    }
    public void SliderValueSet(float value)
    {
        int intVal = (int) value;
        if (intVal == 5)
            sliderLabel.text = "100%";
        else
            sliderLabel.text = $"{ReflectivityManager.GetReflectivityFromSliderValue(intVal) * 100}%";
    }
}
