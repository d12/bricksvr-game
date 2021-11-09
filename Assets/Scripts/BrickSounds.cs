using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSounds : MonoBehaviour
{
    private static BrickSounds _instance;

    public static BrickSounds GetInstance()
    {
        if (_instance == null) _instance = GameObject.FindWithTag("BrickSounds").GetComponent<BrickSounds>();

        return _instance;
    }

    public AudioClip[] brickSnapClips;
    public AudioClip[] brickCarpetClips;
    public AdjustPlayerScale adjustPlayerScale;

    private AudioClip RandomBrickSnapSound()
    {
        return brickSnapClips[Random.Range(0, brickSnapClips.Length)];
    }

    private AudioClip RandomBrickCarpetSound()
    {
        return brickCarpetClips[Random.Range(0, brickCarpetClips.Length)];
    }

    public void PlayBrickSnapSound(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(RandomBrickSnapSound(), pos);
    }

    public void PlayBrickCarpetSound(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(RandomBrickCarpetSound(), pos);
    }
}
