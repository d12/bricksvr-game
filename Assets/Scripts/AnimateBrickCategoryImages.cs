using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateBrickCategoryImages : MonoBehaviour
{
    public bool active;

    private float animationValue; // 0 (inactive), 1 (active)
    public float animationSpeed = 1f;

    public Color unselectedColor;
    public Color selectedColor;

    public float unselectedImageScale;
    public float selectedImageScale;

    public Image backgroundImage;
    public Image brickImage;
    public string categoryName;

    private void Awake()
    {
        animationValue = active ? 1f : 0f;
        RunAnimation();
    }


    // Update is called once per frame
    void Update()
    {
        if ((active && animationValue >= 1f) || (!active && animationValue <= 0f))
            return;

        animationValue += (Time.deltaTime * animationSpeed) * (active ? 1 : -1);

        RunAnimation();
    }

    void RunAnimation()
    {
        backgroundImage.color = Color.Lerp(unselectedColor, selectedColor, animationValue);
        brickImage.transform.localScale =
            Vector3.one * Mathf.SmoothStep(unselectedImageScale, selectedImageScale, animationValue);
    }
}
