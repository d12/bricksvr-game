using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AdjustPlayerScale : MonoBehaviour
{
    public Session _session;
    public AvatarManager avatarManager;

    public JoystickLocomotion joystickLocomotion;
    public Camera headCamera;
    public XRRig xrRig;

    public XRInteractorLineVisual leftMenuLaser;
    public XRInteractorLineVisual rightMenuLaser;
    public Transform brickPickerMenuTransform;
    public Transform rigTransformToScale;
    public ChunkedRenderer chunkedRenderer;
    public MovementVignette movementVignette;

    private float _scale;
    private float _defaultMenuLaserWidth;
    private float _defaultNearClipPlane;
    private Vector3 _defaultMenuScale;


    private static readonly float[] SliderValueToScalePercentages = {
        0.15f,
        0.25f,
        0.5f,
        0.75f,
        1f,
        1.25f,
        1.5f,
        2f,
        3f,
        5f
    };

    private void Awake()
    {
        _defaultMenuLaserWidth = leftMenuLaser.lineWidth;
        _defaultNearClipPlane = headCamera.nearClipPlane;
        _defaultMenuScale = brickPickerMenuTransform.localScale;
    }

    public void Start()
    {
        _session.didSessionStart += DidConnectToRoom;
    }

    public float GetScale()
    {
        return _scale;
    }

    public static float GetScaleFromSliderValue(int sliderValue)
    {
        return SliderValueToScalePercentages[sliderValue - 1];
    }

    public void SetScale(int sliderValue)
    {
        _scale = GetScaleFromSliderValue(sliderValue);
        // We don't call ChangePlayerScale right away because it makes using the slider really hard. We modify the user scale when the world is loaded, and when the misc menu is closed.
    }

    public void ChangePlayerScale()
    {
        if (_session.isPlaying == false)
            return;

        Vector3 scaleVector = new Vector3(_scale, _scale, _scale);

        Vector3 headPositionBefore = xrRig.cameraGameObject.transform.position;
        rigTransformToScale.localScale = scaleVector;
        Vector3 headPositionAfter = xrRig.cameraGameObject.transform.position;

        FixHeadPosition(headPositionBefore, headPositionAfter);
        SetBrickPickerMenuScale();
        SetLaserSize();
        SetCameraNearClipDistance();
        SetChunkedRendererPlayerScale();
        SetVignetteSensitivity();

        joystickLocomotion.playerScaleMultiplier = _scale;

        // Adjust voice pitch?
    }

    // Changing the scale will move the players head, which is extremely uncomfortable. Check how much the head moves when we adjust scale and make an adjustment to the rig position.
    private void FixHeadPosition(Vector3 headPositionBefore, Vector3 headPositionAfter)
    {
        transform.position += headPositionBefore - headPositionAfter;
    }

    private void SetLaserSize()
    {
        leftMenuLaser.lineWidth = _defaultMenuLaserWidth * _scale;
        rightMenuLaser.lineWidth = _defaultMenuLaserWidth * _scale;
    }

    private void SetCameraNearClipDistance()
    {
        headCamera.nearClipPlane = _defaultNearClipPlane * _scale;
    }

    private void SetBrickPickerMenuScale()
    {
        brickPickerMenuTransform.localScale = _defaultMenuScale * _scale;
    }

    private void SetChunkedRendererPlayerScale()
    {
        chunkedRenderer.playerScale = _scale;
    }

    private void SetVignetteSensitivity()
    {
        movementVignette.SetPlayerScale(_scale);
    }

    private void DidConnectToRoom(Session session)
    {
        ChangePlayerScale();
    }
}
