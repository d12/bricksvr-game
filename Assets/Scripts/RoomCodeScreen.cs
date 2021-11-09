using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomCodeScreen : MonoBehaviour
{
    public TextMeshProUGUI text;
    public NormalSessionManager normalSessionManager;
    public GameObject loadingScreen;
    private string _code;

    public void JoinRoom()
    {
        gameObject.SetActive(false);
        loadingScreen.SetActive(true);
        normalSessionManager.JoinRoomWrapper(_code);
    }

    public void SetRoomCode(string code)
    {
        _code = code;
        text.text =
            $"Your room code is <b>{FormatRoomName(code)}</b>. You can also find this code in the room menu.\n\nShare this code with friends to play together!";
    }

    private string FormatRoomName(string roomName)
    {
        if (roomName.Length <= 2)
            return roomName;

        if (roomName.Length <= 4)
            return $"{roomName.Substring(0, 2)} {roomName.Substring(2, (roomName.Length - 2))}";

        if(roomName.Length <= 6)
            return $"{roomName.Substring(0, 2)} {roomName.Substring(2, 2)} {roomName.Substring(4, (roomName.Length - 4))}";

        return $"{roomName.Substring(0, 2)} {roomName.Substring(2, 2)} {roomName.Substring(4, 2)} {roomName.Substring(6, (roomName.Length - 6))}";
    }
}
