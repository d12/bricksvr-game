using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public class UIButtonEvents : MonoBehaviour
{
    public Color textBaseColor;
    public Color textHoveredColor;
    public Color textSelectedColor;

    [FormerlySerializedAs("iconBaseColor")] public Color imageBaseColor;
    [FormerlySerializedAs("iconHoveredColor")] public Color imageHoveredColor;
    public Color imageSelectedColor;

    public TextMeshProUGUI text;
    public Image image;

    public float hoverGrowFactor;

    private Vector3 _initialButtonScale;

    public float animationSpeed;

    private float _animationIndex; // 0f -> 1f
    private bool _hovered;
    private bool _selected;

    private Color _defaultButtonImageColor;
    private Color _selectedButtonImageColor;
    private Color _hoveredButtonImageColor;

    private Button _button;

    private void Awake()
    {
        _initialButtonScale = transform.localScale;
        if (text != null)
            text.color = textBaseColor;

        if(image != null)
            image.color = imageBaseColor;

        _button = GetComponent<Button>();
        _defaultButtonImageColor = _button.colors.normalColor;
        _selectedButtonImageColor = _button.colors.selectedColor;
        _hoveredButtonImageColor = _button.colors.highlightedColor;
    }

    private void Update()
    {
        if (_selected)
            return;

        if (Mathf.Approximately(0f, _animationIndex) && !_hovered)
            return;

        if (Mathf.Approximately(1f, _animationIndex) && _hovered)
            return;

        _animationIndex += animationSpeed * Time.deltaTime * (_hovered ? 1 : -1);
        _animationIndex = Mathf.Clamp01(_animationIndex);

        RunAnimations();
    }

    public void SetSelected(bool selected)
    {
        _selected = selected;

        ColorBlock buttonColors = _button.colors;
        buttonColors.normalColor = selected ? _selectedButtonImageColor : _defaultButtonImageColor;
        buttonColors.highlightedColor = selected ? buttonColors.normalColor : _hoveredButtonImageColor;
        _button.colors = buttonColors;

        if (text != null)
        {
            text.color = selected ? textSelectedColor : textBaseColor;
        }
    }

    private void RunAnimations()
    {
        if(text != null)
            text.color = Color.Lerp(textBaseColor, textHoveredColor, _animationIndex);

        if(image != null)
            image.color = Color.Lerp(imageBaseColor, imageHoveredColor, _animationIndex);

        transform.localScale = _initialButtonScale *
                               Mathf.Lerp(_initialButtonScale.x, _initialButtonScale.x * hoverGrowFactor,
                                   _animationIndex);
    }

    private void OnDisable()
    {
        _animationIndex = 0;
        _hovered = false;

        RunAnimations();
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        _hovered = true;
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        _hovered = false;
    }
}
