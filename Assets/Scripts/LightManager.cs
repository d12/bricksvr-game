using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public enum Area
    {
        Main = 0,
        Tutorial = 1,
    }

    public Light[] lights;
    private Light _activeLight;

    private static LightManager _instance;
    public static LightManager GetInstance()
    {
        if (_instance == null)
            _instance = GameObject.FindWithTag("LightManager").GetComponent<LightManager>();

        return _instance;
    }

    private void Awake()
    {
        foreach (Light l in lights)
        {
            l.enabled = false;
        }

        EnableLight(Area.Main);
    }

    public void EnableLight(Area area)
    {
        if(_activeLight != null)
            _activeLight.enabled = false;

        _activeLight = lights[(int) area];
        _activeLight.enabled = true;
    }
}
