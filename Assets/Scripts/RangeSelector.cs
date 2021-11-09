using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI.ProceduralImage;

public class RangeSelector : MonoBehaviour
{
    public TextMeshProUGUI text;

    public int min;
    public int max;

    private int _currentValue;

    public SerializableIntEvent ValueUpdated;

    [System.Serializable]
    public class SerializableIntEvent : UnityEvent<int> { }

    public void Next()
    {
        if (_currentValue == max)
            _currentValue = min;
        else
            _currentValue += 1;

        UpdateUIText();
        SendValueUpdatedEvent();
    }

    public void Previous()
    {
        if (_currentValue == min)
            _currentValue = max;
        else
            _currentValue -= 1;

        UpdateUIText();
        SendValueUpdatedEvent();
    }

    public void SetValue(int value)
    {
        _currentValue = value;

        UpdateUIText();
    }

    private void SendValueUpdatedEvent()
    {
        ValueUpdated?.Invoke(_currentValue);
    }

    private void UpdateUIText()
    {
        text.text = _currentValue.ToString();
    }
}
