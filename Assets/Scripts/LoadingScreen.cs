using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public NormalSessionManager normalSessionManager;
    public TextMeshProUGUI loadingText;
    public GameObject backButton;
    public RoomCodeScreen roomCodeScreen;

    private Coroutine _createRoomCoroutine;

    public const string NetworkErrorText = "Error: Could not connect to the BricksVR server, are you connected to the internet?";

    public void CreateRoom(string roomName)
    {
        if (_createRoomCoroutine != null)
            return;

        StartCoroutine(CreateRoomIEnum(roomName));
    }

    private IEnumerator CreateRoomIEnum(string roomName)
    {
        loadingText.text = "Status: Creating room...";
        CoroutineWithData isVersionSupported =
            new CoroutineWithData(this, BrickServerInterface.GetInstance().GetIsVersionSupported());
        yield return isVersionSupported.coroutine;

        IsVersionSupportedResponse versionSupportedResponse = (IsVersionSupportedResponse) isVersionSupported.result;

        if (versionSupportedResponse == null || !versionSupportedResponse.supported)
        {
            loadingText.text = versionSupportedResponse == null ? NetworkErrorText : "Error: Your game is out of date! Please update on Steam or the Oculus store.";
            backButton.SetActive(true);
            _createRoomCoroutine = null;
            yield break;
        }

        CoroutineWithData cd = new CoroutineWithData(this, BrickServerInterface.GetInstance().CreateRoom(roomName));
        yield return cd.coroutine;

        CreateRoomResponse response = (CreateRoomResponse) cd.result;
        if (response.connectivityError || response.upstreamError)
        {
            loadingText.text = response.upstreamError
                ? NormalSessionManager.UpstreamErrorText
                : NormalSessionManager.NetworkErrorText;
            backButton.SetActive(true);
        } else if (!string.IsNullOrEmpty(response.error))
        {
            loadingText.text = $"Upstream error: {response.error}";
            backButton.SetActive(true);
        }
        else
        {
            roomCodeScreen.SetRoomCode(response.code);
            roomCodeScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        _createRoomCoroutine = null;
    }
}
