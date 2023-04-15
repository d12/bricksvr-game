using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System.IO;
using TMPro;


public class RecentRoomsManager : MonoBehaviour
{
    public UserSettings userSettings;
    public NormalSessionManager normalSessionManager;

    public GameObject noSavedRoomsObject;
    public GameObject savedRoomsObject;

    public GameObject recentRoomPrefab;
    public GameObject loadingPage;

    private bool _initializedSavedRoomsList;
    private TextInfo _textInfo;

    private const int NumberOfRoomsToLoad = 40;

    public GameObject[] savedRooms;
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
    //     foreach (Transform t in savedRoomsObject.transform)
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
        RenderSavedRoomsList();
    }

    private string[] FindSaves() {
        if(!Directory.Exists($"{Application.dataPath}/saves/"))
            Directory.CreateDirectory($"{Application.dataPath}/saves/");
        
        return Directory.GetFiles($"{Application.dataPath}/saves/")
            .Where(file => file.EndsWith(".bricks")).ToArray();
    }

    private void RenderNoSavedRooms()
    {
        Debug.Log("========= NO SAVED ROOMS =========");
        noSavedRoomsObject.SetActive(true);
        Debug.Log("========= NO SAVED ROOMS =========");
        savedRoomsObject.SetActive(false);
        Debug.Log("========= NO SAVED ROOMS =========");
        pagedScroll.DisableButtons();
        Debug.Log("========= NO SAVED ROOMS =========");
    }

    private void RenderSavedRoomsList()
    {
        noSavedRoomsObject.SetActive(false);
        savedRoomsObject.SetActive(true);

        InitializeSavedRoomsList();
        _initializedSavedRoomsList = true;
    }

    private void InitializeSavedRoomsList()
    {
        string[] rooms = FindSaves();

        if(rooms.Length <= 0) {
            RenderNoSavedRooms();
            return;
        }

        List<string> roomNamesToLoad = new List<string>();

        for (int i = 0; i < Mathf.Min(rooms.Length, NumberOfRoomsToLoad); i++)
        {
            GameObject buttonObject = savedRooms[i];
            if (i < 4) buttonObject.SetActive(true);

            Button button = buttonObject.GetComponent<Button>();

            TextMeshProUGUI roomNameText = recentRoomsNames[i];

            roomNameText.text = rooms[i];

            int i1 = i;
            button.onClick.AddListener(delegate { ButtonClicked(rooms[i1]); });
        }

        pagedScroll.SetFixedElementCount(rooms.Length);
    }

    private void ButtonClicked(string roomName)
    {
        normalSessionManager.JoinRoomWrapper(roomName);
        gameObject.SetActive(false);
        loadingPage.SetActive(true);
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
