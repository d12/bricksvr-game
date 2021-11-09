using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateRenderDistanceSliderText : MonoBehaviour
{
    public TextMeshProUGUI sliderLabel;

    public void SliderValueSet(int value)
    {
        switch (value)
        {
            case 1:
                sliderLabel.text = "1 chunk";
                break;
            case 21:
                sliderLabel.text = "Unlimited";
                break;
            default:
                sliderLabel.text = $"{value} chunks";
                break;
        }
    }
}
