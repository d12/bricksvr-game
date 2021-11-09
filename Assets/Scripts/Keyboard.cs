using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Keyboard sits between the keyboard keys and whatever is consuming the keyboard events. This lets us add new keyboards
// used by different parts of the app without needing to change the events on each key individually.
public class Keyboard : MonoBehaviour
{
    [System.Serializable]
    public class SerializableStringEvent : UnityEvent<string> { }

    public SerializableStringEvent keyPressed;
    public UnityEvent backspacePressed;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayKeySound()
    {
        _audioSource.PlayOneShot(_audioSource.clip);
    }

    public void KeyboardKeyPressed(string key)
    {
        if (key == "backspace")
        {
            backspacePressed.Invoke();
        }
        else
        {
            keyPressed.Invoke(key);
        }
    }
}
