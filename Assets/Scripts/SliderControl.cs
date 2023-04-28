using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SliderControl : MonoBehaviour
{
    public int min;
    public int max;
    public int initialValue;

    private int _value;

    public GameObject progressMarkers;

    public GameObject markerPrefab;

    public Image[] markerImages;

    public Color filledColor;
    public Color unfilledColor;

    public TextMeshProUGUI text;

    private CustomSliderLabels _customSliderLabels;

    public SerializableIntEvent valueUpdated;

    [System.Serializable]
    public class SerializableIntEvent : UnityEvent<int> { }

    // Start is called before the first frame update
    private void Start()
    {
        _value = initialValue;
        _customSliderLabels = GetComponent<CustomSliderLabels>();
        ReRenderSlider();
    }

    public int GetValue()
    {
        return _value;
    }

    public void Increment()
    {
        if (_value < max)
        {
            _value += 1;
            valueUpdated.Invoke(_value);
        }

        ReRenderSlider();
    }

    public void Decrement()
    {
        if (_value > min)
        {
            _value -= 1;
            valueUpdated.Invoke(_value);
        }

        ReRenderSlider();
    }

    public void SetValue(int value)
    {
        int clampedValue = Mathf.Clamp(value, min, max);
        _value = clampedValue;
        valueUpdated.Invoke(_value);
        ReRenderSlider();
    }

    private void ReRenderSlider()
    {
        RecolorSliderMarkers();
        UpdateText();
    }

    private void UpdateText()
    {
        if(text != null)
            text.text = _customSliderLabels == null ? _value.ToString() : _customSliderLabels.LabelFor(_value);
    }

    private void RecolorSliderMarkers()
    {
        for (int i = min; i < max; i++)
        {
            markerImages[i - min].color = i < _value ? filledColor : unfilledColor;
        }
    }

    private void OnValidate()
    {
        #if UNITY_EDITOR
            if (!Application.isEditor)
                return;

            if (max < min)
            {
                Debug.LogError("Slider maximum cannot be less than the minimum");
                return;
            }

            if (markerImages.Length != (max - min))
            {
                foreach (Transform t in progressMarkers.transform)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        if(t != null) DestroyImmediate(t.gameObject);
                    };
                }

                markerImages = new Image[(max - min)];

                for (int i = 0; i < (max - min); i++)
                {
                    markerImages[i] = Instantiate(markerPrefab, progressMarkers.transform).GetComponent<Image>();
                }
            }

            _value = initialValue;
            ReRenderSlider();
        #endif
    }
}
