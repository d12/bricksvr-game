using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdatePlayerScaleSliderText : MonoBehaviour
{
    public TextMeshProUGUI sliderLabel;

    public void SliderValueSet(int value)
    {
        sliderLabel.text = $"{AdjustPlayerScale.GetScaleFromSliderValue(value) * 100}%";
    }
}