using UnityEngine;

[ExecuteInEditMode]
public class SetColorOnPrefabBrick : MonoBehaviour
{
    public Color color;

    private static readonly int Color = Shader.PropertyToID("_Color");
    private static readonly int TexOffset = Shader.PropertyToID("_TexOffset");

    // Start is called before the first frame update
    void Start()
    {
        SetColor(color);
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor(Color, color);
        props.SetFloat(TexOffset, Random.Range(0f, 1f));

        Renderer renderer = GetComponentInChildren<Renderer>();
        if(renderer != null)
            renderer.SetPropertyBlock(props);

        BrickAttach attach = GetComponent<BrickAttach>();
        if (attach != null)
        {
            attach.Color = color;
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        SetColor(color);
    }
    #endif
}
