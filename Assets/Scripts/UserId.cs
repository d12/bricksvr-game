using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserId
{
    public static string Get()
    {
        return SystemInfo.deviceUniqueIdentifier.ToLower();
    }

    public static string GetShortId()
    {
        return Get().Substring(0, 8);
    }
}
