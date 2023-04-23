using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.Android;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public class SessionManager : MonoBehaviour
{
    public GameObject realtimeGameobject;
    private AvatarManager _avatarManager;
    public GameObject playerControllers;

    public GameObject menuPlayerSpawnPoint; // TODO: Should be a transform
    public Transform gameSpawnPoint;

    public GameObject menuLeftHand;
    public GameObject menuRightHand;

    public GameObject teleporterLeftHand;
    public GameObject teleporterRightHand;

    public GameObject inGameLeftHand;
    public GameObject inGameRightHand;

    public GameObject head;

    [FormerlySerializedAs("inGameCodeDisplay1")] public TextMeshProUGUI menuRoomCodeDisplay;

    public GameObject menuEnvironment;
    public GameObject mainEnvironment;

    public Session session;

    public MeshRenderer loadingObjectRenderer;
    private Material _loadingObjectMat;

    public AudioSource ambientMusic;

    public JoystickLocomotion joystickLocomotion;

    public XRInteractionManager xrInteractionManager;

    private BrickStore _brickStore;

    public TextMeshProUGUI menuRoomCode;

    public UserSettings userSettings;

    public GameObject vrRig;

    public ControllerButtonInput buttonInput;

    public GameObject menuBoard;

    public DownloadBricksOnLoad downloadBricksOnLoad;

    private bool _didSessionStart;
    private bool _didSessionEnd;

    private Coroutine _joiningCoroutine;

    // Menu pages
    public GameObject mainPage;
    [FormerlySerializedAs("joinPage")] public GameObject joinByCodePage;
    public GameObject settingsPage;
    public GameObject inGameMain;
    public GameObject audioSettingsPage;
    public GameObject controlsSettingsPage;
    public GameObject miscSettingsPage;
    public GameObject loadingPage;
    public GameObject roomMenuPage;
    public GameObject enterNamePage;
    public GameObject joinMenuPage;
    public GameObject joinRecentPage;
    public GameObject playersMenu;
    public GameObject versionTooOldMenu;
    public GameObject errorMenu;
    public GameObject findFriendsMenu;
    public GameObject avatarEditorMenu;
    public GameObject exportRoomMenu;

    public Toggle teleportToggle;
    public Toggle micEnabledToggle;
    public Toggle otherMicsEnabledToogle;
    public Toggle performanceMode;
    public Toggle musicPlayingToggle;
    public Toggle smoothTurnToggle;
    public Toggle pushToTalkEnabledToggle;
    public Toggle brickClickSoundsToggle;
    public Slider musicVolumeSlider;
    public Slider movementSpeedSlider;
    public Slider rotationSpeedSlider;
    public Slider vignetteSlider;
    [FormerlySerializedAs("renderDistance")] public Slider renderDistanceSlider;
    public Slider playerScaleSlider;
    public Slider brickShininessSlider;

    public BrickPickerManager brickPickerMenu;

    public TextMeshProUGUI joiningStatusText;
    public GameObject joiningBackButton;

    public TutorialManager tutorialManager;

    public MusicPlayer musicPlayer;

    private readonly string _roomPrefix = $"v{ReleaseVersion.MinorString()}-";
    private string _roomName;

    public float _ambientMusicMaxVolume;
    private bool _inGameMenuUp;

    public bool debugMode;
    public bool oldMenuEnabled;

    public GameObject tutorialEnvironment;
    public TextMeshProUGUI roomNameLabel;

    public const string UpstreamErrorText = "Error: Received an error status from the BricksVR datastore.\n\nPlease join the Discord at https://bricksvr.com/discord or wait a few minutes.";
    public const string NetworkErrorText = "Error: Could not connect to the BricksVR server, are you connected to the internet?";

    public MovementVignette movementVignette;
    public AdjustPlayerScale adjustPlayerScale;

    private static SessionManager instance;
    public static SessionManager GetInstance() {
        if(instance == null)
            instance = FindObjectOfType<SessionManager>();
        
        return instance;
    }

    private IEnumerator Start()
    {
        _avatarManager = realtimeGameobject.GetComponent<AvatarManager>();
        _loadingObjectMat = loadingObjectRenderer.material;
        _roomName = "";

        session.didSessionStart += DidSessionStart;
        session.didSessionEnd += DidSessionEnd;

        tutorialEnvironment.SetActive(false);

        WarmResourceLoadCaches();
        BrickId.FetchNewBrickID();

        if (!userSettings.TutorialPlayed)
        {
            tutorialManager.StartTutorial();
            yield break;
        }

        MovePlayerToMainMenu();
        mainEnvironment.SetActive(false);
        OnMenu();

        if (userSettings.Nickname == "")
        {
            mainPage.SetActive(false);
            enterNamePage.SetActive(true);
        }

        joystickLocomotion.enabled = false;
        _brickStore = BrickStore.GetInstance();

        _ambientMusicMaxVolume = userSettings.MusicVolume;
        ambientMusic.volume = 0;

        string error = LoadingError.GetError();
        bool intentionalDisconnect = LoadingError.IntentionalDisconnect;
        if (error != null && !intentionalDisconnect)
        {
            DisableAllMenus();
            errorMenu.SetActive(true);
        }

        LoadingError.IntentionalDisconnect = false;
        LoadingError.ClearError();

        yield return StartCoroutine(ScreenFadeProvider.Unfade(ambientMusic, _ambientMusicMaxVolume));

        if (debugMode)
        {
            CreateAndJoin();
        }
    }

    private void Update()
    {
        if (session.isPlaying && (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.Touch) || Input.GetKeyUp(KeyCode.M)))
        {
            if (!_inGameMenuUp)
            {
                _inGameMenuUp = true;

                DisableAllMenus();
                inGameMain.SetActive(true);

                joystickLocomotion.enabled = false;
                buttonInput.EnableMenuControls();
                menuBoard.SetActive(true);
                MoveMenuToFrontOfUser();

                menuLeftHand.SetActive(false);
                menuRightHand.SetActive(true);

                teleporterLeftHand.SetActive(false);
                teleporterRightHand.SetActive(false);

                _avatarManager.localAvatar.leftHand.gameObject.SetActive(false);
                _avatarManager.localAvatar.rightHand.gameObject.SetActive(false);
            }
            else
            {
                _inGameMenuUp = false;
                adjustPlayerScale.ChangePlayerScale();

                joystickLocomotion.enabled = true;
                buttonInput.DisableMenuControls();
                menuBoard.SetActive(false);

                menuLeftHand.SetActive(false);
                menuRightHand.SetActive(false);

                // teleporterLeftHand.SetActive(userSettings.TeleportTriggersEnabled);
                // teleporterRightHand.SetActive(userSettings.TeleportTriggersEnabled);

                _avatarManager.localAvatar.leftHand.gameObject.SetActive(true);
                _avatarManager.localAvatar.rightHand.gameObject.SetActive(true);
            }
        }
    }

    public string GetRoomName()
    {
        return _roomName;
    }

    public string NormcoreRoomForCode(string roomCode)
    {
        return _roomPrefix + roomCode;
    }

    public bool Playing()
    {
        return session.isPlaying;
    }

    public bool Loading()
    {
        return session.isLoading;
    }

    public bool InGameMenuUp()
    {
        return _inGameMenuUp;
    }

    public void CreateAndJoin()
    {
        if (_joiningCoroutine != null) return;

        _roomName = RandomRoomName();
        _joiningCoroutine = StartCoroutine(JoinRoom());
    }

    public void JoinRoomWrapper()
    {
        StartCoroutine(JoinRoom());
    }

    public void JoinRoomWrapper(string roomName)
    {
        _roomName = roomName;
        StartCoroutine(JoinRoom());
    }

    private IEnumerator JoinRoom()
    {
        joiningBackButton.SetActive(false);
        joiningStatusText.text = "Establishing connection...";

        if (_roomName.Length != 6 && _roomName.Length != 8)
        {
            joiningStatusText.text = "Error: Room not found. Enter a valid room code or create a new room.";
            joiningBackButton.SetActive(true);
            _joiningCoroutine = null;
            yield break;
        }

        //_loading = true;

        _brickStore.ClearAndRemoveFromWorld();

        menuRoomCodeDisplay.text = FormatRoomNameAnyLenNoMono(_roomName);

        CoroutineWithData isVersionSupported =
            new CoroutineWithData(this, BrickServerInterface.GetInstance().GetIsVersionSupported());
        yield return isVersionSupported.coroutine;

        IsVersionSupportedResponse versionSupportedResponse = (IsVersionSupportedResponse) isVersionSupported.result;

        if (versionSupportedResponse == null || !versionSupportedResponse.supported)
        {
            joiningStatusText.text = versionSupportedResponse == null ? NetworkErrorText : "Error: Your game is out of date! Please update on Steam or the Oculus store.";
            joiningBackButton.SetActive(true);
            _joiningCoroutine = null;
            yield break;
        }

        joiningStatusText.text = "Fetching room metadata...";

        CoroutineWithData cd =
            new CoroutineWithData(this, BrickServerInterface.GetInstance().RoomInfo(_roomPrefix + _roomName));
        yield return cd.coroutine;

        RoomInfoResponse response = (RoomInfoResponse) cd.result;
        roomNameLabel.text = RoomDisplayName.DisplayNameForRoomName(response.name);

        if (!response.Exists)
        {
            Debug.LogError("Room doesn't exist!");

            joiningStatusText.text = "Error: Room not found. Enter a valid room code or create a new room.";
            joiningBackButton.SetActive(true);
            _joiningCoroutine = null;
            yield break;
        }

        Debug.Log("Downloading...");
        downloadBricksOnLoad.Reset();
        yield return StartCoroutine(DownloadBricksFromDatastore());

        if (downloadBricksOnLoad.Errored)
        {
            Debug.Log("Errored while downloading!");
            HandleBrickDownloadError();
            _joiningCoroutine = null;
            yield break;
        }

        Debug.Log("Downloading succeeded, moving on");

        menuLeftHand.SetActive(false);
        menuRightHand.SetActive(false);

        joiningStatusText.text = "Status: Joining room...";

        _ambientMusicMaxVolume = ambientMusic.volume;

        yield return StartCoroutine(ScreenFadeProvider.Fade(ambientMusic));

        musicPlayer.Pause();
        Permission.RequestUserPermission(Permission.Microphone);
        session.Connect(_roomPrefix + _roomName);

        float time = Time.time;
        while (Time.time - time < 15f)
        {
            if (_didSessionStart) break;
            yield return null;
        }

        if (!_didSessionStart)
        {
            Debug.Log("Failed to connect to Normcore room.");
            // DidDisconnectFromRoom will move the user back to the main menu
            yield break;
        }

        // Need to wait until we're connected to normcore to do this part
        downloadBricksOnLoad.LoadBricksParentedToHeads();

        userSettings.AddRecentRoom(_roomName);

        WarmOtherCaches();
        yield return brickPickerMenu.WarmMenu();
        BrickColorMap.WarmColorDictionary();
        WarmSpawnerCaches();

        buttonInput.DisableMenuControls();

        menuEnvironment.SetActive(false);
        mainEnvironment.SetActive(true);

        movementVignette.WithVignetteDisabled(() =>
        {
            playerControllers.transform.position = gameSpawnPoint.position;
            playerControllers.transform.rotation = gameSpawnPoint.rotation;

            if (Application.isEditor)
            {
                Vector3 pos = playerControllers.transform.position;
                pos.y -= 0.3f;
                playerControllers.transform.position = pos;
            }
        });

        menuBoard.SetActive(false);

        // Some time for things to settle
        yield return new WaitForSeconds(0.25f);

        //_loading = false;
        musicPlayer.Resume();
        joystickLocomotion.enabled = true;

        yield return StartCoroutine(ScreenFadeProvider.Unfade(ambientMusic, _ambientMusicMaxVolume));
        
        if (!_inGameMenuUp)
        {
            // teleporterLeftHand.SetActive(userSettings.TeleportTriggersEnabled);
            // teleporterRightHand.SetActive(userSettings.TeleportTriggersEnabled);
        }

        loadingObjectRenderer.enabled = false;

        _joiningCoroutine = null;
    }

    public void BackToMenuWrapper()
    {
        LoadingError.IntentionalDisconnect = true;
        UserSettings.GetInstance().TutorialPlayed = true;
        if(session.isSinglePlayer) LocalSessionLoader.SaveRoom(session.saveDirectory);

        StartCoroutine(BackToMenu());
    }

    private void HandleBrickDownloadError()
    {
        if (downloadBricksOnLoad.upstreamError)
        {
            joiningStatusText.text = UpstreamErrorText;
        }
        else
        {
            joiningStatusText.text = NetworkErrorText;
        }

        joiningBackButton.SetActive(true);
    }

    private IEnumerator BackToMenu()
    {
        if (_joiningCoroutine != null) yield break;

        yield return (_joiningCoroutine = StartCoroutine(ScreenFadeProvider.Fade(ambientMusic)));
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void PressJoinGameNumberButton(string button)
    {
        if (button == "<")
        {
            if (_roomName.Length > 0) _roomName = _roomName.Remove(_roomName.Length - 1, 1);
            UpdateNumberOnMenuUI();
            return;
        }

        if (_roomName.Length == 8) return;

        _roomName += button;

        UpdateNumberOnMenuUI();
    }

    public void ResetRoomName()
    {
        _roomName = "";
        UpdateNumberOnMenuUI();
    }

    private void UpdateNumberOnMenuUI()
    {
        if (_roomName.Length == 0)
        {
            menuRoomCode.text = "Enter a room code";
        }
        else
        {
            menuRoomCode.text = FormatRoomName(_roomName);
        }
    }

    public void UpdateSettingsUI()
    {
        teleportToggle.SetIsOnWithoutNotify(userSettings.TeleportTriggersEnabled);
        micEnabledToggle.SetIsOnWithoutNotify(userSettings.MicrophoneEnabled);
        otherMicsEnabledToogle.SetIsOnWithoutNotify(userSettings.OtherPlayersMicsEnabled);
        performanceMode.SetIsOnWithoutNotify(userSettings.SuperUltraPerformanceMode);
        musicPlayingToggle.SetIsOnWithoutNotify(userSettings.PlayMusicEnabled);
        smoothTurnToggle.SetIsOnWithoutNotify(userSettings.SmoothRotationEnabled);
        brickClickSoundsToggle.SetIsOnWithoutNotify(userSettings.BrickClickSoundsEnabled);
        musicVolumeSlider.SetValueWithoutNotify(userSettings.MusicVolume);
        movementSpeedSlider.SetValueWithoutNotify(userSettings.MovementSpeed);
        rotationSpeedSlider.SetValueWithoutNotify(userSettings.RotationSpeed);
        vignetteSlider.SetValueWithoutNotify(userSettings.VignetteStrength);
        renderDistanceSlider.SetValueWithoutNotify(userSettings.RenderDistance);
        playerScaleSlider.SetValueWithoutNotify(userSettings.PlayerScale);
        pushToTalkEnabledToggle.SetIsOnWithoutNotify(userSettings.PushToTalkEnabled);
        brickShininessSlider.SetValueWithoutNotify(userSettings.BrickShininess);
    }

    public void ClearRoomName()
    {
        _roomName = "";
        UpdateNumberOnMenuUI();
    }

    private void OnMenu()
    {
        loadingObjectRenderer.enabled = true;
        menuBoard.SetActive(oldMenuEnabled);
        movementVignette.WithVignetteDisabled(() =>
        {
            vrRig.transform.position = menuPlayerSpawnPoint.transform.position;
        });

        // Switch back to main page
        DisableAllMenus();

        mainPage.SetActive(true);

        // Re-enable menu environment, it gets disabled when we go in-game
        menuEnvironment.SetActive(true);
        ClearRoomName();

        teleporterLeftHand.SetActive(false);
        teleporterRightHand.SetActive(false);
    }

    private void MovePlayerToMainMenu()
    {
        movementVignette.WithVignetteDisabled(() =>
        {
            playerControllers.transform.position = menuPlayerSpawnPoint.transform.position;
            playerControllers.transform.rotation = menuPlayerSpawnPoint.transform.rotation;
        });
    }

    private IEnumerator DownloadBricksFromDatastore()
    {
        downloadBricksOnLoad.StartLoading(_roomPrefix + _roomName, joiningStatusText);

        // TODO: Make it so we can't get stuck here if something goes wrong.
        yield return new WaitUntil(() => downloadBricksOnLoad.isDoneDownloading);
    }

    private void WarmResourceLoadCaches()
    {
        List<BrickData.Brick> bricks = BrickData.AllBricks();
        foreach (BrickData.Brick brick in bricks)
        {
            GameObject brickObj = Resources.Load(brick.PrefabName) as GameObject;
            GameObject placedBrickObj = Resources.Load(brick.PrefabName + " - Placed") as GameObject;
        }
    }

    public void WarmOtherCaches()
    {
        GameObject brick4x2 = Instantiate(Resources.Load<GameObject>("4x2"), new Vector3(0, -10, 0), Quaternion.identity);
        GameObject brick2x2 = Instantiate(Resources.Load<GameObject>("2x2"), new Vector3(0, -20, 0), Quaternion.identity);
        GameObject brick1x4 = Instantiate(Resources.Load<GameObject>("1x4"), new Vector3(0, -30, 0), Quaternion.identity);
        GameObject brick4x2Placed = GameObject.Instantiate(Resources.Load<GameObject>("4x2 - Placed"), new Vector3(0, -40, 0), Quaternion.identity);
        GameObject brick2x2Placed = GameObject.Instantiate(Resources.Load<GameObject>("2x2 - Placed"), new Vector3(0, -50, 0), Quaternion.identity);
        GameObject brick1x4Placed = GameObject.Instantiate(Resources.Load<GameObject>("1x4 - Placed"), new Vector3(0, -60, 0), Quaternion.identity);

        brick4x2.GetComponent<ShowSnappableBrickPositions>().enabled  = true; // Warm ShowSnappableBrickPositions update logic, mainly brick attach detection code

        BrickDestroyer destroyer = BrickDestroyer.GetInstance();

        destroyer.DelayedDestroy(brick2x2);
        destroyer.DelayedDestroy(brick1x4);
        destroyer.DelayedDestroy(brick4x2Placed);
        destroyer.DelayedDestroy(brick2x2Placed);
        destroyer.DelayedDestroy(brick1x4Placed);

        // To warm the XRInteractionManager update loop (which is SLOW the first time), force select an object, wait 3 frames to allow Update() to be called
        // then destroy it.
        xrInteractionManager.ForceSelect(playerControllers.GetComponentInChildren<XRDirectInteractor>(), brick4x2.GetComponent<XRGrabInteractable>());
        Wait.ForFrames(3, () =>
        {
            _brickStore.Delete(brick4x2.GetComponent<BrickAttach>().GetUuid());
            destroyer.DelayedDestroy(brick4x2);
        });
    }

    private void WarmSpawnerCaches()
    {
        // TODO: Reimplement this
        // GameObject brick = brickSpawner.CreateBrick();
        // _brickStore.Delete(brick.GetComponent<BrickAttach>().GetUuid());
        // StartCoroutine(DelayedDestroyer.DestroyRealtime(brick));
    }

    private void DidSessionStart(Session session)
    {
        _didSessionStart = true;
        GameObject localHead = AvatarManager.GetInstance().localAvatar.head.gameObject;
        Renderer[] localHeadRenderers = localHead.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in localHeadRenderers)
        {
            r.enabled = false;
        }

        menuLeftHand.SetActive(false);
        menuRightHand.SetActive(false);
    }

    private void DidSessionEnd(Session session)
    {
        _didSessionEnd = true;
        LoadingError.SetError("Disconnected from room");
        StartCoroutine(BackToMenu());
    }

    private string RandomRoomName()
    {
        return (Random.Range(100000, 999999)).ToString();
    }

    private void MoveMenuToFrontOfUser()
    {
        float verticalOffset = session.isPlaying ? 0.2f : 1.4f;
        Vector3 gazeDirection = head.transform.forward;
        gazeDirection.y = 0f;
        Vector3 headPosition = head.transform.position;
        menuBoard.transform.position = headPosition + (gazeDirection.normalized * (session.isPlaying ? 2.8f : 10f));
        menuBoard.transform.rotation = Quaternion.LookRotation(menuBoard.transform.position - headPosition);
        menuBoard.transform.position += new Vector3(0, verticalOffset, 0);
    }

    private Coroutine _tutorialCoroutine;
    public void PlayTutorial()
    {
        if (_tutorialCoroutine == null)
            _tutorialCoroutine = StartCoroutine(PlayTutorialIEnum());
    }

    private IEnumerator PlayTutorialIEnum()
    {
        userSettings.TutorialPlayed = false;
        yield return StartCoroutine(ScreenFadeProvider.Fade(ambientMusic));
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    private void DisableAllMenus()
    {
        mainPage.SetActive(false);
        joinByCodePage.SetActive(false);
        settingsPage.SetActive(false);
        inGameMain.SetActive(false);
        audioSettingsPage.SetActive(false);
        controlsSettingsPage.SetActive(false);
        miscSettingsPage.SetActive(false);
        loadingPage.SetActive(false);
        roomMenuPage.SetActive(false);
        enterNamePage.SetActive(false);
        joinMenuPage.SetActive(false);
        joinRecentPage.SetActive(false);
        playersMenu.SetActive(false);
        findFriendsMenu.SetActive(false);
        avatarEditorMenu.SetActive(false);
        exportRoomMenu.SetActive(false);
    }

    private string PadRoomNameWithUnderscores(string roomName)
    {
        return $"{roomName}{String.Concat(Enumerable.Repeat("_", 8 - roomName.Length))}";
    }

    private string FormatRoomName(string roomName)
    {
        // if (roomName.Length <= 2)
        //     return roomName;
        //
        // if (roomName.Length <= 4)
        //     return $"{roomName.Substring(0, 2)} {roomName.Substring(2, (roomName.Length - 2))}";
        //
        // if(roomName.Length <= 6)
        //     return $"{roomName.Substring(0, 2)} {roomName.Substring(2, 2)} {roomName.Substring(4, (roomName.Length - 4))}";

        string paddedName = PadRoomNameWithUnderscores(roomName);
        return $"<mspace=0.6em>{paddedName.Substring(0, 2)} {paddedName.Substring(2, 2)} {paddedName.Substring(4, 2)} {paddedName.Substring(6, 2)}</mspace>";
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
