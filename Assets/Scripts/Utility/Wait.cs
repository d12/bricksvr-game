using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : MonoBehaviour
{
    private static Wait _instance;
    // Start is called before the first frame update
    private static Wait GetInstance()
    {
        if (_instance != null) return _instance;

        _instance = GameObject.Find("Waiter").GetComponent<Wait>();
        return _instance;
    }

    public static void ForFrames(int frames, Action function)
    {
        GetInstance().ForFramesInstance(frames, function);
    }

    public static void Until(Func<bool> until, Action function)
    {
        GetInstance().UntilInstance(until, function);
    }

    private void ForFramesInstance(int frames, Action function)
    {
        StartCoroutine(ForFramesIEnum(frames, function));
    }

    private void UntilInstance(Func<bool> until, Action function)
    {
        StartCoroutine(UntilIEnum(until, function));
    }

    private static IEnumerator ForFramesIEnum(int frames, Action function)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return 0;
        }

        function();
    }

    public static IEnumerator UntilIEnum(Func<bool> until, Action function)
    {
        yield return new WaitUntil(until);

        function();
    }
}
