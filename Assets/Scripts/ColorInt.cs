using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ColorInt
{
    private readonly int _colorInt;
    private Color _cachedColor;
    private bool _cachedColorSet;

    public ColorInt(int colorInt)
    {
        _colorInt = colorInt;
    }

    public ColorInt(int r, int g, int b, int a)
    {
        _colorInt = (a & 0xff) << 24 | (r & 0xff) << 16 | (g & 0xff) << 8 | (b & 0xff);
    }

    public ColorInt(int r, int g, int b)
    {
        _colorInt = (255 & 0xff) << 24 | (r & 0xff) << 16 | (g & 0xff) << 8 | (b & 0xff);
    }

    public ColorInt(Color32 color)
    {
        _colorInt = _colorInt = (color.a & 0xff) << 24 | (color.r & 0xff) << 16 | (color.g & 0xff) << 8 | (color.b & 0xff);
    }

    public int GetInt()
    {
        return _colorInt;
    }

    public Color32 GetColor32()
    {
        return IntToColor32(_colorInt);
    }

    public Color GetColor()
    {
        if (!_cachedColorSet)
        {
            _cachedColor = GetColor32();
            _cachedColorSet = true;
        }

        return _cachedColor;
    }

    public static Color32 IntToColor32(int color)
    {
        int a = (color >> 24) & 0xff;
        int r = (color >> 16) & 0xff;
        int g = (color >>  8) & 0xff;
        int b = (color      ) & 0xff;

        return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
    }

    public static int ColorToInt(Color color)
    {
        byte a = (byte) (color.a * 255);
        byte r = (byte) (color.r * 255);
        byte g = (byte) (color.g * 255);
        byte b = (byte) (color.b * 255);

        return (a & 0xff) << 24 | (r & 0xff) << 16 | (g & 0xff) << 8 | (b & 0xff);
    }

    public static int Color32ToInt(Color32 color)
    {
        return (color.a & 0xff) << 24 | (color.r & 0xff) << 16 | (color.g & 0xff) << 8 | (color.b & 0xff);
    }
}
