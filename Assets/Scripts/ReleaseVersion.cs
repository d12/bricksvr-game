using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReleaseVersion
{
    public static int Major = 0;
    public static int Minor = 2;
    public static int Patch = 42;

    public static string VersionString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }

    public static string MinorString()
    {
        return $"{Major}.{Minor}";
    }
}
