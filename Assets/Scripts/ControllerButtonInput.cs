using UnityEngine;

public class ControllerButtonInput : MonoBehaviour
{
    public GameObject realtimeObject;
    public GameObject leftMenuHand;
    public GameObject rightMenuHand;

    private OVRInput.Controller _activeController;

    private Session _session;
    private bool _reset;
    private bool inMenu = true;

    // Start is called before the first frame update
    public void Start()
    {
        _session = realtimeObject.GetComponent<Session>();
        _reset = false;

        leftMenuHand.SetActive(true);
        rightMenuHand.SetActive(false);
        _activeController = OVRInput.Controller.LTouch;
    }

    // Update is called once per frame
    private void Update()
    {
        if ((!_session.isPlaying && !_session.isLoading) || inMenu)
        {
            MenuLogic();
        }
        else if (!_reset)
        {
            ResetMenuState();
        }
    }

    public void EnableMenuControls()
    {
        _reset = false;
        inMenu = true;
    }

    public void DisableMenuControls()
    {
        inMenu = false;
    }

    private void MenuLogic()
    {
    //     if (_activeController == OVRInput.Controller.None)
    //     {
    //         OVRInput.Controller controller = OVRInput.GetActiveController();
    //         if (controller == OVRInput.Controller.Touch || controller == OVRInput.Controller.RTouch)
    //         {
    //             _activeController = OVRInput.Controller.RTouch;
    //             rightMenuHand.SetActive(true);
    //             leftMenuHand.SetActive(false);
    //         }
    //         else if (controller == OVRInput.Controller.LTouch)
    //         {
    //             _activeController = OVRInput.Controller.LTouch;
    //             leftMenuHand.SetActive(true);
    //             rightMenuHand.SetActive(false);
    //         }
    //         else
    //         {
    //             _activeController = OVRInput.Controller.RTouch;
    //             leftMenuHand.SetActive(false);
    //             rightMenuHand.SetActive(true);
    //         }
    //     }

        // Switch laser if we press the opposite trigger
        if (_activeController == OVRInput.Controller.LTouch && ((OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) > 0.5f) || Input.GetMouseButtonDown(1)))
        {
            _activeController = OVRInput.Controller.RTouch;

            rightMenuHand.SetActive(true);
            leftMenuHand.SetActive(false);
        }
        else if (_activeController == OVRInput.Controller.RTouch && ((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) > 0.5f) || Input.GetMouseButtonDown(1)))
        {
            _activeController = OVRInput.Controller.LTouch;

            rightMenuHand.SetActive(false);
            leftMenuHand.SetActive(true);
        }
    }

    private void ResetMenuState()
    {
        // _activeController = OVRInput.Controller.None;
        _reset = true;
    }
}
