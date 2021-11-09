using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class RoomDisplayName
{
    public static string DisplayNameForRoomName(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
            return "Unnamed Room";

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(roomName.ToLower());
    }
}
