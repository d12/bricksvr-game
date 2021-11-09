using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ColorPickerSaveSpot : MonoBehaviour
{
    [FormerlySerializedAs("border")] public Image borderImage;
    [FormerlySerializedAs("color")] public Image colorImage;

    public BrickPickerManager brickPickerManager;

    public Color color = Color.white;

    private void Awake()
    {
        colorImage.color = color;
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        colorImage.color = color;
    }

    private void OnTriggerEnter(Collider c)
    {
        if (!brickPickerManager.IsMenuFullyOpen)
            return;

        brickPickerManager.SaveSpotSelected(this);
        Enable();
    }

    public void Enable()
    {
        borderImage.enabled = true;
    }

    public void Disable()
    {
        borderImage.enabled = false;
    }
}
