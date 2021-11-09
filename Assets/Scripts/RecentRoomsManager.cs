using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecentRoomsManager : MonoBehaviour
{
    public UserSettings userSettings;
    public NormalSessionManager normalSessionManager;

    public GameObject noRecentRoomsObject;
    public GameObject recentRoomsObject;

    public GameObject recentRoomPrefab;
    public GameObject loadingPage;

    private bool _initializedRecentRoomsList;
    private TextInfo _textInfo;

    private const int NumberOfRoomsToLoad = 40;

    public GameObject[] recentRooms;
    public TextMeshProUGUI[] recentRoomsRoomCodes;
    public TextMeshProUGUI[] recentRoomsNames;
    public TextMeshProUGUI[] recentRoomsBrickCounts;

    public PagedScroll pagedScroll;

    // private void OnValidate()
    // {
    //     List<TextMeshProUGUI> roomCodes = new List<TextMeshProUGUI>();
    //     List<TextMeshProUGUI> names = new List<TextMeshProUGUI>();
    //     List<TextMeshProUGUI> brickCounts = new List<TextMeshProUGUI>();
    //
    //     foreach (Transform t in recentRoomsObject.transform)
    //     {
    //
    //         GameObject room = t.gameObject;
    //         roomCodes.Add(room.transform.Find("RoomCode").GetComponent<TextMeshProUGUI>());
    //         names.Add(room.transform.Find("Name").GetComponent<TextMeshProUGUI>());
    //         brickCounts.Add(room.transform.Find("BrickCount").GetComponent<TextMeshProUGUI>());
    //     }
    //
    //     recentRoomsRoomCodes = roomCodes.ToArray();
    //     recentRoomsNames = names.ToArray();
    //     recentRoomsBrickCounts = brickCounts.ToArray();
    // }

    private void OnEnable()
    {
        _textInfo = new CultureInfo("en-US", false).TextInfo;
        if (userSettings.RecentRooms == "")
        {
            RenderNoRecentRooms();
        }
        else
        {
            RenderRecentRoomsList();
        }
    }

    private void RenderNoRecentRooms()
    {
        noRecentRoomsObject.SetActive(true);
        recentRoomsObject.SetActive(false);
        pagedScroll.DisableButtons();
    }

    private void RenderRecentRoomsList()
    {
        noRecentRoomsObject.SetActive(false);
        recentRoomsObject.SetActive(true);

        if (!_initializedRecentRoomsList)
        {
            InitializeRecentRoomsList();
            _initializedRecentRoomsList = true;
        }
    }

    private void InitializeRecentRoomsList()
    {
        string[] rooms = userSettings.RecentRooms.Split(',');

        List<(string, (TextMeshProUGUI, TextMeshProUGUI))> roomNamesToLoad = new List<(string, (TextMeshProUGUI, TextMeshProUGUI))>();

        for (int i = 0; i < Mathf.Min(rooms.Length, NumberOfRoomsToLoad); i++)
        {
            GameObject buttonObject = recentRooms[i];
            if (i < 4) buttonObject.SetActive(true);

            Button button = buttonObject.GetComponent<Button>();

            TextMeshProUGUI roomCodeText = recentRoomsRoomCodes[i];
            TextMeshProUGUI roomNameText = recentRoomsNames[i];
            TextMeshProUGUI brickCountText = recentRoomsBrickCounts[i];

            roomCodeText.text = FormatRoomNameAnyLenNoMono(rooms[i]);
            roomNamesToLoad.Add((normalSessionManager.NormcoreRoomForCode(rooms[i]), (roomNameText, brickCountText)));

            int i1 = i;
            button.onClick.AddListener(delegate { ButtonClicked(rooms[i1]); });
        }

        pagedScroll.SetFixedElementCount(rooms.Length);

        if(roomNamesToLoad.Count > 0)
            StartCoroutine(LoadRoomNames(roomNamesToLoad));
    }

    private void ButtonClicked(string roomName)
    {
        normalSessionManager.JoinRoomWrapper(roomName);
        gameObject.SetActive(false);
        loadingPage.SetActive(true);
    }

    private IEnumerator LoadRoomNames(List<(string, (TextMeshProUGUI, TextMeshProUGUI))> roomNamesToLoad)
    {
        yield return 0; // Wait a frame, this frame is already busy

        // Loads room names in serial. Loading in parallel causes GCP to spin up a node for each room which causes ~5 second delay in load time.
        foreach ((string roomName, (TextMeshProUGUI nameLabel, TextMeshProUGUI brickCountLabel)) in roomNamesToLoad)
        {
            yield return LoadRoomName(roomName, nameLabel, brickCountLabel);
        }
    }

    private IEnumerator LoadRoomName(string roomCode, TextMeshProUGUI roomNameText, TextMeshProUGUI brickCountLabel)
    {
        CoroutineWithData cd =
            new CoroutineWithData(this, BrickServerInterface.GetInstance().RoomInfo(roomCode));
        yield return cd.coroutine;

        RoomInfoResponse response = (RoomInfoResponse) cd.result;
        if (response.error)
        {
            roomNameText.text = "Failed to load name";
        }
        else
        {
            roomNameText.text = RoomDisplayName.DisplayNameForRoomName(response.name);
            brickCountLabel.text = BrickStringFromResponse(response.brickCount);
        }

        roomNameText.fontSize = 0.3f;
    }

    private string BrickStringFromResponse(string brickCountResponse)
    {
        if (brickCountResponse == "0" || brickCountResponse == null || brickCountResponse == "")
            return "0 bricks";

        try
        {
            int numberOfBricks = Convert.ToInt32(brickCountResponse);

            return numberOfBricks == 1 ? "1 brick" : $"{numberOfBricks:n0} bricks";
        }
        catch (FormatException)
        {
            return "0 bricks";
        }
    }

    private string TitleCase(string roomName)
    {
        return _textInfo.ToTitleCase(roomName.ToLower());
    }

    private string FormatRoomNameAnyLenNoMono(string roomName)
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
