using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCategoryColors : MonoBehaviour
{
    public Image[] categoryImages;
    public Color defaultEditorColor;

    public void UpdateColor(Color c)
    {
        foreach (Image i in categoryImages)
            i.color = c;
    }

    private void OnValidate()
    {
        if (!Application.isEditor) return;
        List<Image> images = new List<Image>();
        foreach (Transform t in transform)
        {
            foreach (Transform t1 in t)
            {
                Image i = t1.gameObject.GetComponentInChildren<Image>();
                if(i != null)
                    images.Add(i);
            }
        }

        categoryImages = images.ToArray();
        UpdateColor(defaultEditorColor);
    }
}
