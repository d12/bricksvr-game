using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AvatarFaceGetter : MonoBehaviour
{
    [FormerlySerializedAs("faces")] public Sprite[] mouths;
    public Sprite[] eyes;

    private static AvatarFaceGetter _instance;

    public static AvatarFaceGetter GetInstance()
    {
        if (_instance == null)
            _instance = GameObject.FindWithTag("AvatarFaceGetter").GetComponent<AvatarFaceGetter>();

        return _instance;
    }

    // begins with 1
    public Sprite GetMouth(int index)
    {
        return mouths[index - 1];
    }

    // begins with 1
    public Sprite GetEyes(int index)
    {
        return eyes[index - 1];
    }
}
