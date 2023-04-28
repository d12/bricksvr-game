using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ColorCarousel : MonoBehaviour
{
    public Image colorImage;
    public Color[] colors;
    private int[] colorInts;
    private bool colorIntsInitialized;

    [FormerlySerializedAs("initialColorIndex")] public int editorColorIndex;

    private int _currentColorIndex;

    public SerializableIntEvent ColorUpdated;

    [System.Serializable]
    public class SerializableIntEvent : UnityEvent<int> { }

    private void Awake()
    {
        if(!colorIntsInitialized)
            InitializeColorInts();
    }

    private void InitializeColorInts()
    {
        colorInts = new int[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colorInts[i] = ColorInt.ColorToInt(colors[i]);
        }

        colorIntsInitialized = true;
    }

    public void NextColor()
    {
        if (_currentColorIndex == colors.Length - 1)
            _currentColorIndex = 0;
        else
            _currentColorIndex += 1;

        ReRenderColor();
        SendColorUpdatedEvent();
    }

    public void PreviousColor()
    {
        if (_currentColorIndex == 0)
            _currentColorIndex = colors.Length - 1;
        else
            _currentColorIndex -= 1;

        ReRenderColor();
        SendColorUpdatedEvent();
    }

    public void SelectColor(int colorInt)
    {
        if(!colorIntsInitialized)
            InitializeColorInts();

        int index = Array.IndexOf(colorInts, colorInt);
        if (index != -1)
            _currentColorIndex = index;

        ReRenderColor();
    }

    private void SendColorUpdatedEvent()
    {
        ColorUpdated?.Invoke(ColorInt.ColorToInt(colors[_currentColorIndex]));
    }

    private void ReRenderColor()
    {
        colorImage.color = colors[_currentColorIndex];
    }

    private void OnValidate()
    {
        if (!Application.isEditor)
            return;

        editorColorIndex = Mathf.Clamp(editorColorIndex, 0, colors.Length - 1);
        colorImage.color = colors[editorColorIndex];
    }
}
