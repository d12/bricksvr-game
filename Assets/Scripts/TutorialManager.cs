using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject playerControllers;
    public GameObject infoBoard;
    public GameObject tutorialEnvContents;

    public SnapTurnProvider snapTurnProvider;
    public SmoothTurn smoothTurn;

    public BrickPickerManager brickPickerManager;

    public AudioSource ambientMusic;
    private float _ambientMusicMaxVolume;

    public JoystickLocomotion joystickLocomotion;
    public Renderer loadingObjectRenderer;

    public HandBrickSpawner leftHandBrickSpawner;
    public HandBrickSpawner rightHandBrickSpawner;

    public GameObject menuLeftHand;
    public GameObject menuRightHand;

    public GameObject teleporterLeftHand;
    public GameObject teleporterRightHand;

    public ControllerButtonInput buttonInput;

    public AudioSource canvasAudioSource;

    public XRRig xrRig;

    public PalletteBrickCollider palletteBrickCollider;

    public Button roomMenuButton;

    private GameObject firstBrick;
    private GameObject secondBrick;

    [Header("Reference points")]
    public GameObject tutorialSpawnPoint;
    public GameObject firstBrickSpawnPoint;
    public GameObject secondBrickSpawnPoint;

    [Header("Text")]
    public TextMeshProUGUI firstTextBox;
    public TextMeshProUGUI secondTextBox;
    public TextMeshProUGUI thirdTextBox;
    public TextMeshProUGUI fourthTextBox;
    public TextMeshProUGUI fifthTextBox;
    public TextMeshProUGUI sixthTextBox;
    public TextMeshProUGUI seventhTextBox;
    public TextMeshProUGUI seventhAndAHalfthTextBox;
    public TextMeshProUGUI eighthTextBox;
    public TextMeshProUGUI ninthTextBox;
    public TextMeshProUGUI lastTextBox;

    private bool _startedTutorial;
    private bool _advancing;
    private bool _connectedToNormcore;

    private bool _didConnectToRoom;
    private bool _didDisconnectFromRoom;

    private int _initialLateralMovementIndex;
    private int _initialVerticalMovementIndex;
    private int _initialSnapTurnIndex;
    private int _initialBrickMenuIndex;
    private int _numberOfBricks;
    private int _initialHandBrickSpawnerIndex;
    private int _initialPalletteBrickColliderIndex;

    private Coroutine _startTutorialCoroutine;
    private Action _updateAction;

    private static TutorialManager _instance;

    public MovementVignette movementVignette;

    public static TutorialManager GetInstance()
    {
        if (_instance == null) _instance = FindObjectOfType<TutorialManager>();
        return _instance;
    }

    private void Start()
    {
        _instance = this;
        //realtime.didConnectToRoom += DidConnectToRoom;
        //realtime.didDisconnectFromRoom += DidDisconnectFromRoom;
    }

    private void Update()
    {
        _updateAction?.Invoke();
    }

    public void StartTutorial()
    {
        if(_startTutorialCoroutine == null)
            _startTutorialCoroutine = StartCoroutine(StartTutorialIEnum());

        Debug.Log($"Quality level: {QualitySettings.GetQualityLevel()}");
    }

    public bool IsTutorialRunning()
    {
        return _startedTutorial;
    }

    private IEnumerator StartTutorialIEnum()
    {
        _startedTutorial = true;

        _ambientMusicMaxVolume = UserSettings.GetInstance().MusicVolume;
        ambientMusic.volume = 0;

        tutorialEnvContents.SetActive(true);

        MovePlayerToTutorial();
        yield return ConnectToNormcore();

        if (!_didConnectToRoom)
            yield break;

        _connectedToNormcore = true;

        snapTurnProvider.enabled = true;
        smoothTurn.enabled = false;

        joystickLocomotion.enabled = true;

        menuLeftHand.SetActive(false);
        menuRightHand.SetActive(false);

        teleporterLeftHand.SetActive(false);
        teleporterRightHand.SetActive(false);

        buttonInput.DisableMenuControls();

        roomMenuButton.interactable = false;

        firstBrick = CreateNewBrick("4x2", firstBrickSpawnPoint.transform, new Color32(118, 85, 227, 255));
        yield return brickPickerManager.WarmMenu();

        LightManager.GetInstance().EnableLight(LightManager.Area.Tutorial);

        yield return null;

        yield return ScreenFadeProvider.Unfade(ambientMusic, _ambientMusicMaxVolume);

        loadingObjectRenderer.enabled = false;

        _updateAction = UpdateFirstPart;

        _startTutorialCoroutine = null;
    }

    private void MovePlayerToTutorial()
    {
        movementVignette.WithVignetteDisabled(() =>
        {
            xrRig.MoveCameraToWorldLocation(tutorialSpawnPoint.transform.position);

            xrRig.RotateAroundCameraUsingRigUp(tutorialSpawnPoint.transform.eulerAngles.y);
        });
    }

    private IEnumerator ConnectToNormcore()
    {
        //realtime.Connect("tutorial-" + UserId.Get() + Random.Range(0, 100000));

        float time = Time.time;
        while (Time.time - time < 10f)
        {
            if (_didConnectToRoom) break;
            yield return null;
        }

        if (!_didConnectToRoom)
        {
            Debug.LogError("Failed to connect to Normcore!");
            UserSettings.GetInstance().TutorialPlayed = true;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    private void PlaySuccessTrack()
    {
        canvasAudioSource.Play();
    }

    private GameObject CreateNewBrick(string prefabName, Transform t, Color color)
    {
        GameObject brick = Instantiate(Resources.Load<GameObject>(prefabName), t.position, t.rotation);

        BrickAttach attach = brick.GetComponent<BrickAttach>();
        attach.Color = color;
        attach.SetUuid(BrickId.FetchNewBrickID());

        EnablePhysicsOnBrick(brick);

        return brick;
    }

    private void EnablePhysicsOnBrick(GameObject brick)
    {
        GameObject.Destroy(brick.GetComponent<AutoDespawnPhysicsBricks>()); // Don't want this brick despawning
        brick.GetComponent<Rigidbody>().isKinematic = false;
    }

    private IEnumerator AnimateBetweenTextObjectsWithCross(TextMeshProUGUI oldText, TextMeshProUGUI newText)
    {
        float fadeSpeed = 1.8f;

        oldText.fontStyle = FontStyles.Strikethrough;

        yield return new WaitForSeconds(0.8f);

        float alpha = 0f;

        Color oldTextColor = oldText.color;
        Color newTextColor = newText.color;

        newText.enabled = true;

        do
        {
            oldTextColor.a = 1 - alpha;
            oldText.color = oldTextColor;

            newTextColor.a = alpha;
            newText.color = newTextColor;

            alpha += (0.01f * fadeSpeed);

            yield return null;
        } while (alpha < 1f);

        oldText.enabled = false;
    }

    /*
     * PART 1, Attach a brick
     */

    private void UpdateFirstPart()
    {
        if (!_advancing && firstBrick == null)
        {
            _advancing = true;
            StartCoroutine(MoveToSecondStep());
        }
    }

    private IEnumerator MoveToSecondStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(firstTextBox, secondTextBox);

        secondBrick = CreateNewBrick("2x2", secondBrickSpawnPoint.transform, new Color32(227, 118, 85, 255));

        _advancing = false;
        _updateAction = UpdateSecondPart;
    }

    /*
     *  PART 2, Place brick again
     */

    private void UpdateSecondPart()
    {
        if (!_advancing && secondBrick == null)
        {
            _advancing = true;
            StartCoroutine(MoveToThirdStep());
        }
    }

    private IEnumerator MoveToThirdStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(secondTextBox, thirdTextBox);

        _advancing = false;
        _updateAction = UpdateThirdPart;
        _initialLateralMovementIndex = joystickLocomotion.lateralMovementIndex;
    }

    /*
     * PART 3, Joystick movement
     */

    private void UpdateThirdPart()
    {
        if(!_advancing && ((joystickLocomotion.lateralMovementIndex != _initialLateralMovementIndex) || (Input.GetKey(KeyCode.L))))
        {
            _advancing = true;
            StartCoroutine(MoveToFourthStep());
        }
    }

    private IEnumerator MoveToFourthStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(thirdTextBox, fourthTextBox);
        _advancing = false;
        _updateAction = UpdateFourthPart;
        _initialVerticalMovementIndex = joystickLocomotion.verticalMovementIndex;
    }

    /*
     * Part 4: Vertical movement
     */

    private void UpdateFourthPart()
    {
        if(!_advancing && ((joystickLocomotion.verticalMovementIndex != _initialVerticalMovementIndex) || (Input.GetKey(KeyCode.V))))
        {
            _advancing = true;
            StartCoroutine(MoveToFifthStep());
        }
    }

    private IEnumerator MoveToFifthStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(fourthTextBox, fifthTextBox);
        _advancing = false;
        _updateAction = UpdateFifthPart;
        _initialSnapTurnIndex = snapTurnProvider.turnIndex;
    }

    /*
     * Part 5: Rotation
     */

    private void UpdateFifthPart()
    {
        if(!_advancing && ((snapTurnProvider.turnIndex != _initialSnapTurnIndex) || (Input.GetKey(KeyCode.R))))
        {
            _advancing = true;
            StartCoroutine(MoveToSixthStep());
        }
    }

    private IEnumerator MoveToSixthStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(fifthTextBox, sixthTextBox);
        _advancing = false;
        _updateAction = UpdateSixthPart;
        _initialBrickMenuIndex = brickPickerManager.openMenuIndex;
    }

    /*
     * Part 6: Brick menu
     */

    private void UpdateSixthPart()
    {
        if(!_advancing && (brickPickerManager.openMenuIndex != _initialBrickMenuIndex))
        {
            _advancing = true;
            StartCoroutine(MoveToSeventhStep());
        }
    }

    private IEnumerator MoveToSeventhStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(sixthTextBox, seventhTextBox);
        _advancing = false;
        _updateAction = UpdateSeventhPart;
        _numberOfBricks = GameObject.FindObjectsOfType<BrickAttach>().Length;
    }

    /*
     * Part 7: Colors
     */

    private void UpdateSeventhPart()
    {
        if(!_advancing && (GameObject.FindObjectsOfType<BrickAttach>().Length >= (_numberOfBricks + 3)))
        {
            _advancing = true;
            StartCoroutine(MoveFromSeventhToSeventhAndAHalfthStep());
        }
    }

    private IEnumerator MoveFromSeventhToSeventhAndAHalfthStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(seventhTextBox, seventhAndAHalfthTextBox);
        _advancing = false;
        _updateAction = UpdateSeventhAndAHalfthPart;
        _initialPalletteBrickColliderIndex = palletteBrickCollider.collisionIndex;
    }

    /*
     * Part 7.5: Color copy
     */

    private void UpdateSeventhAndAHalfthPart()
    {
        if(!_advancing && palletteBrickCollider.collisionIndex > _initialPalletteBrickColliderIndex)
        {
            _advancing = true;
            StartCoroutine(MoveFromSeventhAndAHalfthToEighthStep());
        }
    }

    private IEnumerator MoveFromSeventhAndAHalfthToEighthStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(seventhAndAHalfthTextBox, eighthTextBox);
        _advancing = false;
        _updateAction = UpdateEighthPart;
        _initialHandBrickSpawnerIndex = leftHandBrickSpawner.spawnerSetIndex + rightHandBrickSpawner.spawnerSetIndex;
    }

    /*
     * Part 8: Hand spawner set
     */

    private void UpdateEighthPart()
    {
        if(!_advancing && ((leftHandBrickSpawner.spawnerSetIndex + rightHandBrickSpawner.spawnerSetIndex > _initialHandBrickSpawnerIndex) || Input.GetKey(KeyCode.S)))
        {
            _advancing = true;
            StartCoroutine(MoveFromEighthToNinthStep());
        }
    }

    private IEnumerator MoveFromEighthToNinthStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(eighthTextBox, ninthTextBox);
        _advancing = false;
        _updateAction = UpdateNinthPart;
        _numberOfBricks = GameObject.FindObjectsOfType<BrickAttach>().Length;
    }

    /*
     * Part 9: Hand spawner spawn
     */

    private void UpdateNinthPart()
    {
        if(!_advancing && (GameObject.FindObjectsOfType<BrickAttach>().Length >= (_numberOfBricks + 3)))
        {
            _advancing = true;
            StartCoroutine(MoveToLastStep());
        }
    }

    private IEnumerator MoveToLastStep()
    {
        PlaySuccessTrack();
        yield return AnimateBetweenTextObjectsWithCross(ninthTextBox, lastTextBox);

        UserSettings.GetInstance().TutorialPlayed = true;

        _advancing = false;
        _updateAction = null;
    }
}
