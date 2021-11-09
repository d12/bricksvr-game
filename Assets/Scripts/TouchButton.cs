using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TouchButton : MonoBehaviour
{
    private bool _isPressed;
    private int _numberOfObjectsPressing;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_isPressed && _button.interactable) _button.OnPointerDown(new PointerEventData(EventSystem.current));
        _isPressed = true;
        _numberOfObjectsPressing += 1;
    }

    private void OnTriggerExit(Collider other)
    {
        _numberOfObjectsPressing -= 1;
        if (_numberOfObjectsPressing != 0) return;

        _isPressed = false;
        if (!_button.interactable) return;

        _button.OnPointerUp(new PointerEventData(EventSystem.current));
        _button.onClick.Invoke();
    }
}
