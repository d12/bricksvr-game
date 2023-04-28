using System.Collections;
using UnityEngine;
using System;

public class Session : MonoBehaviour
{
    public ChunkedRenderer chunkedRenderer;
    public LoadingScreen loadingScreen;
    public string worldName = "My world";
    public string saveDirectory;

    public int clientID {
        get {
            return -1;
        }
    }

    public bool canPlace {
        get {

            if (isMultiPlayer)
                return false;
            else if (isSinglePlayer) return true;
            else if (isTutorial) return true;
            else return false;
        }
    }

    private bool playing;
    public bool isPlaying {
        get {
            return playing;
        }
    }

    private bool _loading;
    public bool isLoading {
        get {
            return _loading;
        }
    }

    private bool _singlePlayer;
    public bool isSinglePlayer {
        get {
            return _singlePlayer;
        }
    }

    public bool isMultiPlayer {
        get {
            return false;
        }
    }

    public bool isTutorial
    {
        get
        {
            return TutorialManager.GetInstance().IsTutorialRunning();
        }
    }

    public event Action<Session> didSessionStart;
    public event Action<Session> didSessionEnd;

    public void Connect(string ip) {
        throw new NotImplementedException();
    }

    public IEnumerator LoadSave(string file) {
        SessionManager manager = SessionManager.GetInstance();
        BrickPrefabCache prefabCache = BrickPrefabCache.GetInstance();
        _singlePlayer = true;
        _loading = true;

        loadingScreen.loadingText.text = $"Status: Generating brick cache...";
        prefabCache.GenerateCache();
        loadingScreen.loadingText.text = $"Status: Reading bricks from file...";
        BrickData.LocalBrickData[] brickData = LocalSessionLoader.ReadSave(file);
        BrickAttach[] createdBricks = new BrickAttach[brickData.Length];

        chunkedRenderer.enabled = false;
        for (int i = 0; i < brickData.Length; i++)
        {
            try
            {
                createdBricks[i] = PlacedBrickCreator.CreateFromBrickObject(brickData[i], false)
                    .GetComponent<BrickAttach>();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed To load a brick {brickData[i].type}");
                Debug.LogException(e);
            }

            if ((createdBricks[i] != null) && (i % 8) == 0)
            {
                yield return null;
                loadingScreen.loadingText.text = $"Status: Loaded {i + 1}/{brickData.Length} bricks...";
            }
        }

        for (int i = 0; i < brickData.Length; i++)
        {
            if (createdBricks[i] == null)
                continue;

            createdBricks[i].RecalculateEnabledConnectors();
            createdBricks[i].RecalculateRenderedGeometry();

            yield return null;
            loadingScreen.loadingText.text = $"Status: Optimized {i + 1}/{brickData.Length} bricks...";
        }

        chunkedRenderer.enabled = true;

        yield return StartCoroutine(ScreenFadeProvider.Fade(manager.ambientMusic));

        manager.musicPlayer.Pause();

        manager.WarmOtherCaches();
        yield return manager.brickPickerMenu.WarmMenu();
        BrickColorMap.WarmColorDictionary();
        //manager.WarmSpawnerCaches();

        manager.buttonInput.DisableMenuControls();
        
        manager.menuEnvironment.SetActive(false);
        manager.mainEnvironment.SetActive(true);

        manager.movementVignette.WithVignetteDisabled(() =>
        {
            manager.playerControllers.transform.position = manager.gameSpawnPoint.position;
            manager.playerControllers.transform.rotation = manager.gameSpawnPoint.rotation;

            if (Application.isEditor)
            {
                Vector3 pos = manager.playerControllers.transform.position;
                pos.y -= 0.3f;
                manager.playerControllers.transform.position = pos;
            }
        });

        manager.menuBoard.SetActive(false);

        manager.menuLeftHand.SetActive(false);
        manager.menuRightHand.SetActive(false);

        // Some time for things to settle
        yield return new WaitForSeconds(0.25f);

        _loading = false;
        manager.musicPlayer.Resume();
        manager.joystickLocomotion.enabled = true;

        yield return StartCoroutine(ScreenFadeProvider.Unfade(manager.ambientMusic, manager._ambientMusicMaxVolume));

        saveDirectory = file;

        playing = true;
        AvatarManager.GetInstance().Initialize(this);
        didSessionStart.Invoke(this);
    }

    public void StartTutorialSession() {
        SessionManager manager = SessionManager.GetInstance();
        manager.joystickLocomotion.enabled = true;
        _singlePlayer = false;
        _loading = false;
        playing = true;
        
        manager.menuLeftHand.SetActive(false);
        manager.menuRightHand.SetActive(false);

        AvatarManager.GetInstance().Initialize(this);
        didSessionStart.Invoke(this);
    }

    public void EndSession()
    {
        playing = false;
        didSessionEnd.Invoke(this);
    }
}
