using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetHandColors : MonoBehaviour
{
    [SerializeField]
    private Color color1;

    [SerializeField]
    private Color color2;

    private static readonly int ColorProp = Shader.PropertyToID("_Color");

    // Start is called before the first frame update
    private void Awake()
    {
        SetPropertyBlocksOnRenderer();
    }

    public void SetPrimaryColor(Color newColor)
    {
        color2 = newColor;
        SetPropertyBlocksOnRenderer();
    }

    public void SetSecondaryColor(Color newColor)
    {
        color1 = newColor;
        SetPropertyBlocksOnRenderer();
    }

    private void SetPropertyBlocksOnRenderer()
    {
        MaterialPropertyBlock props1 = new MaterialPropertyBlock();
        props1.SetColor(ColorProp, color1);

        MaterialPropertyBlock props2 = new MaterialPropertyBlock();
        props2.SetColor(ColorProp, color2);

        Renderer renderer = GetComponent<Renderer>();
        renderer.SetPropertyBlock(props1, 0);
        renderer.SetPropertyBlock(props2, 1);
    }
}
