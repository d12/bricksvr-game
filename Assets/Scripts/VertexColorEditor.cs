using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexColorEditor : MonoBehaviour
{
    public Color color;

    private Color _lastColor;
    private Color32 _lastColor32;

    // Start is called before the first frame update
    void Start()
    {
        _lastColor = color;
        _lastColor32 = color;
        UpdateVertexColors();
    }

    // Update is called once per frame
    void Update()
    {
        if (color != _lastColor)
        {
            _lastColor = color;
            _lastColor32 = color;
            UpdateVertexColors();
        }
    }

    private void UpdateVertexColors()
    {
        Debug.Log("Updating vertex colors!");
        GetComponent<BrickAttach>().Color = _lastColor32;
    }
}
