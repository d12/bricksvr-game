using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenFadeProvider
{
    private static readonly float FadeSpeed = 1f;

    public static IEnumerator Fade(AudioSource music)
    {
        GameObject loadingCube = GameObject.FindWithTag("LoadingCube");
        Renderer loadingRenderer = loadingCube.GetComponent<Renderer>();
        loadingRenderer.enabled = true;
        Material loadingMaterial = loadingRenderer.material;
        Color loadingMatColor = loadingMaterial.color;

        float alpha = 0f;
        float maxVolume = music.volume;

        do
        {
            loadingMatColor.a = alpha;
            loadingMaterial.color = loadingMatColor;

            alpha += (0.01f * FadeSpeed);
            music.volume = maxVolume * (1 - alpha);

            yield return null;
        } while (alpha < 1f);
    }

    public static IEnumerator Unfade(AudioSource music, float maxVolume, bool modifySound = true)
    {
        GameObject loadingCube = GameObject.FindWithTag("LoadingCube");
        Material loadingMaterial = loadingCube.GetComponent<Renderer>().material;
        Color loadingMatColor = loadingMaterial.color;

        float alpha = 1f;

        do
        {
            loadingMatColor.a = alpha;
            loadingMaterial.color = loadingMatColor;

            alpha -= (0.01f * FadeSpeed);
            if(modifySound) music.volume = maxVolume * (1 - alpha);

            yield return null;
        } while (alpha > 0f);

        if(modifySound) music.volume = maxVolume;
    }
}
