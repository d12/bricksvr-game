using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnOffControl : MonoBehaviour
{
    public UIButtonEvents onButtonEvents;
    public UIButtonEvents offButtonEvents;

    public SerializableBoolEvent valueUpdated;

    [System.Serializable]
    public class SerializableBoolEvent : UnityEvent<bool> { }

    private bool _on;
    private bool _initialized;

    // Start is called before the first frame update
    void Start()
    {
        _on = true;
        _initialized = true;
        UpdateButtonStates();
    }

    void OnEnable()
    {
        if(_initialized)
            UpdateButtonStates();
    }

    public void ToggleChanged(bool on)
    {
        if (_on == on)
            return;

        _on = on;
        valueUpdated.Invoke(on);
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        onButtonEvents.SetSelected(_on);
        offButtonEvents.SetSelected(!_on);
    }
}
