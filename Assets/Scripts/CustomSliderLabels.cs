using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSliderLabels : MonoBehaviour
{
    public string[] labels;
    public bool refreshLabels;

    private SliderControl _sliderControl;

    private void Awake()
    {
        _sliderControl = GetComponent<SliderControl>();
    }

    void OnValidate()
    {
        if (!Application.isEditor)
            return;

        if (labels != null && !refreshLabels)
            return;

        refreshLabels = false;

        SliderControl sliderControl = GetComponent<SliderControl>();
        int min = sliderControl.min;
        int max = sliderControl.max;

        labels = new string[max - min + 1];
        for (int i = 0; i < max - min + 1; i++)
        {
            labels[i] = (i + min).ToString();
        }
    }

    public string LabelFor(int value)
    {
        return labels[value - _sliderControl.min];
    }
}
