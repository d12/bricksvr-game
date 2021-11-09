using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BrickColorMap
{
    private static Dictionary<int, Color32> _idsToColors;
    private static Dictionary<Color32, int> _colorsToIds;

    public static Color32 ColorFromID(int id)
    {
        if (_idsToColors == null) InitializeDictionary();

        return _idsToColors[id];
    }

    public static int IDFromColor(Color32 color)
    {
        if (_colorsToIds == null) InitializeDictionary();

        return _colorsToIds[color];
    }

    public static void WarmColorDictionary()
    {
        InitializeDictionary();
    }

    private static void InitializeDictionary()
    {
        _idsToColors = new Dictionary<int, Color32>()
        {
            {0, new Color32(34, 138, 221, 255)}, // bright-blue
            {1, new Color32(58, 159, 73, 255)}, // bright-green
            {2, new Color32(238, 129, 0, 255)}, // bright-orange
            {3, new Color32(224, 75, 144, 255)}, // bright-purple
            {4, new Color32(219, 26, 18, 255)}, // bright-red
            {5, new Color32(141, 183, 49, 255)}, // bright-yel-green
            {6, new Color32(236, 208, 39, 255)}, // bright-yellow
            {7, new Color32(10, 113, 161, 255)}, // dark-azur
            {8, new Color32(0, 0, 0, 0)}, // ghost
            {9, new Color32(209, 128, 86, 255)}, // nougat
            {10, new Color32(108, 50, 15, 255)}, // reddish-brown
            {11, new Color32(0, 0, 1, 255)}, // spr-yellowish-green
            {12, new Color32(222, 222, 222, 255)}, // white
            {13, new Color32(0, 0, 0, 255)}, // black
            {14, new Color32(138, 138, 138, 255)}, // gray
        };

        _colorsToIds = _idsToColors.ToDictionary((i) => i.Value, (i) => i.Key);
    }
}
