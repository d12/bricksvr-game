using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;

public class BrickPickerManager : MonoBehaviour
{
    public Button bricksTabButton;
    public Button tilesTabButton;
    public Button platesTabButton;
    public Button slopesTabButton;
    public Button sideStudsTabButton;
    public Button miscTabButton;

    public GameObject bricksTabObject;
    public GameObject tilesTabObject;
    public GameObject platesTabObject;
    public GameObject slopesTabObject;
    public GameObject sideStudsTabObject;
    public GameObject miscTabObject;

    public GameObject menuContentsObject;

    public GameObject tilePrefab;

    public SessionManager SessionManager;// Find info about the in-game menu. We shouldn't open the brick menu if the in-game menu is open.

    public GameObject leftHand;
    public GameObject rightHand;

    public Slider hueSlider;
    public Slider saturationSlider;
    public Slider valueSlider;

    public ChangeCategoryColors changeCategoryColors;

    public ColorPickerSaveSpot[] colorPickerSaveSpots;
    private ColorPickerSaveSpot _activeColorPickerSaveSpot;

    private Dictionary<string, MenuTab> _tabs;

    private List<BrickPickerBrick> _brickPickerBricks;

    private string _currentTab = "bricks";

    private bool _holdingMenu;
    private bool _holdingMenuWithLeftHand;

    private float _menuRotVel;

    private bool _waitingToReleaseLeftButton;
    private bool _waitingToReleaseRightButton;

    private bool _menuClosing;

    public int openMenuIndex;

    private bool _firstOpen = true;

    public XRGrabInteractable[] sliderInteractbles;
    public XRGrabInteractable categoryInteractable;
    private FadeBrickMenu fade;

    public bool IsMenuOpen => menuContentsObject.transform.localScale.magnitude > 0.001f;
    public bool IsMenuFullyOpen => menuContentsObject.transform.localScale.magnitude > 1.7f;
    private List<(BrickPickerBrick brickPickerBrick, float distance)> hoveredBricksThisFrame = new List<(BrickPickerBrick brickPickerBrick, float distance)>();

    // Start is called before the first frame update
    private void Start()
    {
        fade = menuContentsObject.GetComponent<FadeBrickMenu>();
        _tabs = new Dictionary<string, MenuTab>()
        {
            {"bricks", new MenuTab(bricksTabButton, bricksTabObject)},
            {"tiles", new MenuTab(tilesTabButton, tilesTabObject)},
            {"plates", new MenuTab(platesTabButton, platesTabObject)},
            {"slopes", new MenuTab(slopesTabButton, slopesTabObject)},
            {"side_studs", new MenuTab(sideStudsTabButton, sideStudsTabObject)},
            {"misc", new MenuTab(miscTabButton, miscTabObject)},
        };

        _brickPickerBricks = new List<BrickPickerBrick>();

        _activeColorPickerSaveSpot = colorPickerSaveSpots[0];
        _activeColorPickerSaveSpot.Enable();

        EnableTab(_currentTab);

        SetColor(_activeColorPickerSaveSpot.GetColor());

        fade.BeginShrink();
    }

    private void LateUpdate()
    {
        if (!SessionManager.Playing() || SessionManager.InGameMenuUp()) return;

        if(_holdingMenu)
            RepositionMenu(_holdingMenuWithLeftHand);

        if (_waitingToReleaseLeftButton && (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch) || Input.GetKey(KeyCode.B))) return;
        if (_waitingToReleaseRightButton && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Touch)) return;

        _waitingToReleaseLeftButton = false;
        _waitingToReleaseRightButton = false;

        bool leftJoystickDown;

        // Not currently holding the menu
        if (!_holdingMenu && (
                (leftJoystickDown = (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch)) || Input.GetKey(KeyCode.B)) ||
                OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Touch)
        ))
        {
            ToggleMenu(leftJoystickDown);
            if (IsMenuOpen && !_menuClosing)
            {
                _holdingMenuWithLeftHand = leftJoystickDown;
                _holdingMenu = true;
            }

            switch (leftJoystickDown)
            {
                case true:
                    _waitingToReleaseLeftButton = true;
                    break;
                case false:
                    _waitingToReleaseRightButton = true;
                    break;
            }
        } else if (_holdingMenu && (
            (_holdingMenuWithLeftHand && (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch) || !Input.GetKey(KeyCode.B))) ||
            (!_holdingMenuWithLeftHand && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Touch))))
        {
            _holdingMenu = false;
        }

        ProcessHoveredTiles();
    }

    private void ProcessHoveredTiles()
    {
        if (hoveredBricksThisFrame.Count == 0)
            return;

        float minDistance = 10000f;
        int closestBrickIndex = 0;
        for (int i = 0; i < hoveredBricksThisFrame.Count; i++)
        {
            if (hoveredBricksThisFrame[i].distance < minDistance)
            {
                minDistance = hoveredBricksThisFrame[i].distance;
                closestBrickIndex = i;
            }
        }

        for (int i = 0; i < hoveredBricksThisFrame.Count; i++)
        {
            hoveredBricksThisFrame[i].brickPickerBrick.SetIsHovered(i == closestBrickIndex);
        }

        hoveredBricksThisFrame.Clear();
    }

    public void AddHoveredTile(BrickPickerBrick brickPickerBrick, float distance)
    {
        hoveredBricksThisFrame.Add((brickPickerBrick, distance));
    }

    public void SetColor(Color color)
    {
        _activeColorPickerSaveSpot.SetColor(color);
        changeCategoryColors.UpdateColor(color);
        foreach (BrickPickerBrick brickPickerBrick in _brickPickerBricks)
        {
            brickPickerBrick.SetColor(color);
        }
    }

    public void SaveSpotSelected(ColorPickerSaveSpot saveSpot)
    {
        if (_activeColorPickerSaveSpot) _activeColorPickerSaveSpot.Disable();
        _activeColorPickerSaveSpot = saveSpot;

        Color c = _activeColorPickerSaveSpot.GetColor();

        foreach (BrickPickerBrick brickPickerBrick in _brickPickerBricks)
        {
            brickPickerBrick.SetColor(c);
        }

        SetSliders(c);
    }

    public void SetSliders(Color c)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        hueSlider.value = h;
        saturationSlider.value = s;
        valueSlider.value = v;
    }

    private void SetCategorySliderInteractableActive(bool active)
    {
        categoryInteractable.interactionLayerMask = active ? ~0 : 0;
    }

    private void SetColorSlidersInteractableActive(bool active)
    {
        foreach (XRGrabInteractable interactable in sliderInteractbles)
        {
            interactable.interactionLayerMask = active ? ~0 : 0;
        }
    }

    public IEnumerator WarmMenu()
    {
        SetSliders(_activeColorPickerSaveSpot.GetColor());

        foreach ((string tabName, MenuTab tab) in _tabs.Select(x => (x.Key, x.Value)))
        {
            InitializeTab(tabName);
            EnableTab(tabName);
        }

        yield return 0;
        yield return 0;

        foreach ((string tabName, MenuTab tab) in _tabs.Select(x => (x.Key, x.Value)))
        {
            DisableTab(tabName);
        }

        EnableTab(_currentTab);
    }

    private void ToggleMenu(bool usingLeftHand)
    {
        if (!IsMenuOpen)
        {
            SetInteractablesActive(true);
            _menuClosing = false;
            transform.rotation = (usingLeftHand ? leftHand : rightHand).transform.rotation;

            // We always ignore z rotation on the menu
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            RepositionMenu(usingLeftHand);
            // if (_firstOpen)
            // {
            //     SetSliders(_activeColorPickerSaveSpot.GetColor());
            //     _firstOpen = false;
            // }
            fade.BeginGrow();
        }
        else
        {
            SetInteractablesActive(false);

            fade.BeginShrink();
            _menuClosing = true;
        }

        openMenuIndex += 1;
    }

    private void SetInteractablesActive(bool value)
    {
        SetCategorySliderInteractableActive(value);
        SetColorSlidersInteractableActive(value);
    }

    private void RepositionMenu(bool usingLeftHand)
    {
        GameObject handToUse = usingLeftHand ? leftHand : rightHand;
        Vector3 handPos = handToUse.transform.position;

        transform.position = handPos;

        Quaternion targetRotation = handToUse.transform.rotation;

        // Remove z rotation on target
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0f);

        float delta = Quaternion.Angle(transform.rotation, targetRotation);
        if (delta > 0f)
        {
            float t = Mathf.SmoothDampAngle(delta, 0.0f, ref _menuRotVel, 0.2f);
            t = 1.0f - (t / delta);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }
    }

    public void TabClicked(string tabName)
    {
        if (_currentTab == tabName) return;

        DisableTab(_currentTab);

        EnableTab(tabName);

        _currentTab = tabName;
    }

    private void EnableTab(string tabName)
    {
        MenuTab menuTab = _tabs[tabName];

        menuTab.Gameobject.SetActive(true);
        // if (!menuTab.Initialized)
        // {
        //     InitializeTab(tabName);
        //     menuTab.Initialized = true;
        //     _tabs[tabName] = menuTab;
        // }
    }

    private void DisableTab(string tabName)
    {
        _tabs[tabName].Gameobject.SetActive(false);
    }

    private void InitializeTab(string tabName)
    {
        GameObject tabObject = _tabs[tabName].Gameobject;
        List<BrickData.Brick> bricksForCategory = BrickData.BricksForCategory(tabName);

        foreach (BrickData.Brick brick in bricksForCategory)
        {
            GameObject brickObject = GameObject.Instantiate(tilePrefab, tabObject.transform);
            BrickPickerBrick brickPickerBrick = brickObject.GetComponent<BrickPickerBrick>();
            _brickPickerBricks.Add(brickPickerBrick);
            brickPickerBrick.Initialize(brick, _activeColorPickerSaveSpot.GetColor(), this);
        }
    }

    private struct MenuTab
    {
        public MenuTab(Button tabButton, GameObject gameObject)
        {
            TabButton = tabButton;
            Gameobject = gameObject;
            Initialized = false;
        }

        public readonly Button TabButton;
        public readonly GameObject Gameobject;
        public bool Initialized;
    }
}
