using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCQualitySettings : MonoBehaviour
{
    [Tooltip("0 = quest, 1 = low, 2 = medium, 3 = high")]
    [SerializeField] private int QualitySetting;

    [Tooltip("low = 6, mid = 12, high = 20 + extreme = 25?")]
    [SerializeField] private int shadowDistance;

    [Tooltip("low = 2x, mid = 4x, high = 4x + extreme = 8x")]
    [SerializeField] private int antiAliasing;

    [Tooltip("low = high, mid = vry high, high = very high + very low = low")]
    [SerializeField] private ShadowResolution shadowResolution;

    [Tooltip("low = shadowmask, mid = shadowmask, high = distance shadowmask")]
    [SerializeField] private ShadowmaskMode shadowMask;

    void Start()
    {
        // 0 = quest, 1 = low, 2 = medium, 3 = high
        if (Application.platform == RuntimePlatform.Android)
		{
            QualitySettings.SetQualityLevel(0);
        }
        else QualitySettings.SetQualityLevel(QualitySetting);

        //Anything here needs to display 'custom' as preset

        // low = 6, mid = 12, high = 20 + extreme = 25?
        QualitySettings.shadowDistance = shadowDistance;

        // low = high, mid = very high, high = very high + very low = medium
        QualitySettings.shadowResolution = shadowResolution;

        // low = 2x, mid = 4x, high = 4x + extreme = 8x
        QualitySettings.antiAliasing = antiAliasing;

        // low = shadowmask, mid = shadowmask, high = distance shadowmask
        QualitySettings.shadowmaskMode = shadowMask;


    }

    // 0 = quest, 1 = low, 2 = medium, 3 = high
    public void ChangeQuality(int level)
	{
        QualitySettings.SetQualityLevel(level);
	}
}
