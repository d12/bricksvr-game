using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public UserSettings settings;
    public RangeSelector selector;

    public List<Material> skyboxes;
    public List<Color> fogs;

    public Material defaultBox;
    public Color defaultFog;

    private UnityAction<int> action;

    public void Start()
    {
        action += SkyboxUpdated;
        settings.SkyboxUpdated.AddListener(action);

        selector.SetValue(settings.Skybox);
    }

    public void SkyboxUpdated(int value)
    {
        Material material = skyboxes[value];
        Color fog = fogs[value];

        RenderSettings.skybox = material == null ? defaultBox : material;
        RenderSettings.fogColor = fog == null ? defaultFog : fog;
    }
}
