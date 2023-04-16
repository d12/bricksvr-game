using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using System.Linq;
using System;

public class BrickAttachDetector : MonoBehaviour
{
    [FormerlySerializedAs("_isBeingHeld")] public bool isBeingHeld = false;
    [FormerlySerializedAs("_isAttached")] public bool isAttached = false;

    public GameObject maleConnectorParent;
    public GameObject femaleConnectorParent;

    public List<GameObject> _maleConnectors;
    public List<GameObject> _femaleConnectors;

    public GameObject model;

    private Vector3 _collisionExtents;
    private Vector3 _colliderOffset;

    private Transform _transform;

    private HapticsManager _hapticsManager;

    private XRGrabInteractable _xrGrabInteractable;
    private OwnedPhysicsBricksStore _ownedPhysicsBricksStore;

    public bool skipGrabCallbacks;

    public bool tile;
    public bool window;

    public BoxCollider[] colliders;
    private bool _usingBuiltInColliders;

    public float heightOverride;

    private void Awake()
    {
        // We use the collider on the root game object for detecting if we're hitting any other bricks during brick placement.
        // If we need any other colliders for physics purposes, put them on a child object and add them to Collider[] colliders.
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        _collisionExtents = Vector3.Scale(boxCollider.size / 2, transform.lossyScale); // div by 2 because CheckBox expects halfextents.
        _xrGrabInteractable = GetComponent<XRGrabInteractable>();
        _ownedPhysicsBricksStore = OwnedPhysicsBricksStore.GetInstance();
        _colliderOffset = boxCollider.center;
        foreach(BoxCollider c in colliders)
            if (c.gameObject == gameObject)
                _usingBuiltInColliders = true;
    }

    private void Start()
    {
        _transform = transform;
        _hapticsManager = HapticsManager.GetInstance();
    }

    private void OnEnable()
    {
        GetComponent<XRBaseInteractable>().onSelectEnter.AddListener(BrickGrabbed);
        GetComponent<XRBaseInteractable>().onSelectExit.AddListener(BrickReleased);
    }

    private void OnDisable()
    {
        GetComponent<XRBaseInteractable>()?.onSelectEnter.RemoveListener(BrickGrabbed);
        GetComponent<XRBaseInteractable>().onSelectExit.RemoveListener(BrickReleased);
    }

    private void BrickGrabbed(XRBaseInteractor interactor)
    {
        if (skipGrabCallbacks) return;
            
        foreach (Collider c in colliders)
        {
            c.isTrigger = true;
        }

        GetComponent<Rigidbody>().isKinematic = false;

        isBeingHeld = true;
        isAttached = false;

        GetComponent<ShowSnappableBrickPositions>().enabled = true;
        _ownedPhysicsBricksStore.AddBrick(gameObject);
    }

    private void BrickReleased(XRBaseInteractor interactor)
    {
        if (skipGrabCallbacks) return;
        if (!isBeingHeld) return;

        isBeingHeld = false;

        (bool canConnect, Vector3 newPos, Quaternion newRot, Vector3 connectionDirection) = CheckIfCanConnect();
        if (canConnect)
        {
            try
            {
                if (GetComponent<BrickAttach>().ConnectBricks(newPos, newRot, connectionDirection))
                {
                    bool leftHand = interactor.transform.parent.gameObject.name == "LeftHand";
                    _hapticsManager.PlayHaptics(0.5f, 0.5f, 0.1f, !leftHand, leftHand);
                }
            }
            catch (Exception e)
            {
                Debug.Log("SOMETHING EXPLODED!");
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);

                EnableGravityIfUnowned();
            }
        }
        else
        {
            // GetComponent<BuildingBrickSync>().AddBrickBelow("ground");
            EnableGravityIfUnowned();
        }

        XRGrabInteractable ourInteractable = GetComponent<XRGrabInteractable>();

        GetComponent<XRGrabInteractable>().interactionManager.ForceHoverExit(interactor, ourInteractable);

        foreach (Collider c in colliders)
        {
            c.enabled = true;
            c.isTrigger = false;
        }

        GetComponent<ShowSnappableBrickPositions>().ResetAndDisable();
    }

    private void EnableGravityIfUnowned()
    {
        Wait.ForFrames(2, () =>
        {
            if (!this) return;
            if (_xrGrabInteractable.isSelected) return;

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
        });
    }

    private readonly (bool, Vector3, Quaternion, Vector3) _nullResponse = (false, Vector3.zero, Quaternion.identity, Vector3.zero);
    // This is kind of gross
    public (bool canConnect, Vector3 pos, Quaternion rot, Vector3 connectionDirection) CheckIfCanConnect()
    {
        if (!SessionManager.GetInstance().session.canPlace)
            return _nullResponse;

        GameObject[] femaleConnectorsWithConnections = GetFemaleConnectorsWithConnections();
        GameObject[] maleConnectorsWithConnections = GetMaleConnectorsWithConnections();

        GameObject[] validFemaleConnectors = ValidConnections(femaleConnectorsWithConnections);
        GameObject[] validMaleConnectors = ValidConnections(maleConnectorsWithConnections);

        GameObject[] connectorsToUse = validFemaleConnectors.Length > validMaleConnectors.Length
            ? validFemaleConnectors
            : validMaleConnectors;

        bool connectingDownwards = connectorsToUse == validFemaleConnectors;

        if (connectorsToUse.Length > 0)
        {
            Transform otherBrickTransform = ClosestConnectorFromConnector(connectorsToUse[0]).transform.parent.transform.parent;
            GameObject otherBrick = otherBrickTransform.gameObject; // TODO: Make this fetch multiple otherBricks if we have multiple femaleConnector parents
            Vector3 otherBrickOriginalRot = otherBrick.transform.rotation.eulerAngles;

            if (!IsVectorApproximatelyOne(otherBrickTransform.lossyScale))
            {
                // Scale is not 1, don't attach a brick to it
                return _nullResponse;
            }

            // if (!AngleApproximatelyZero(otherBrickOriginalRot.x) || !AngleApproximatelyZero(otherBrickOriginalRot.z))
            //     return _nullResponse; // We don't currently attach to base bricks that are not on a flat surface.

            Vector3 brickCurrentPos = _transform.position;
            Quaternion brickCurrentRot = _transform.rotation;

            _transform.parent = otherBrick.transform;
            Vector3 oldEulerAngles = otherBrick.transform.rotation.eulerAngles;

            GameObject otherBrickConnector = ClosestConnectorFromConnector(connectorsToUse[0]);
            Transform otherConnectorTransform = otherBrickConnector.transform;

            otherBrick.transform.eulerAngles += new Vector3(-oldEulerAngles.x - otherConnectorTransform.localEulerAngles.x, 0, -oldEulerAngles.z - otherConnectorTransform.localEulerAngles.z);

            Quaternion rot = GetNewRot(otherBrickConnector);
            Vector3 pos = GetNewPosWithRot(rot, otherBrick, connectorsToUse, connectingDownwards);

            _transform.position = pos;
            _transform.rotation = rot;

            otherBrick.transform.eulerAngles = otherBrickOriginalRot;

            Quaternion adjustedRot = _transform.rotation;
            Vector3 adjustedPos = _transform.position;

            bool collidesWithBricks = CollidesWithBricks(adjustedRot, adjustedPos);

            _transform.position = brickCurrentPos;
            _transform.rotation = brickCurrentRot;

            _transform.parent = null;

            if (collidesWithBricks)
            {
                return _nullResponse;
            }

            // For some reason, the math works out with the object rotated 180 degrees. So flip it back.
            adjustedRot *= Quaternion.Euler(0, 180f, 0);

            return (true, adjustedPos, adjustedRot, connectingDownwards ? Vector3.down : Vector3.up);
        }

        return _nullResponse;
    }

    private void OnDrawGizmos()
    {
        foreach (BoxCollider c in colliders)
        {
            Gizmos.DrawWireCube(_transform.position - (_transform.position - _transform.TransformPoint(c.center)),
                Vector3.Scale(c.size, c.transform.lossyScale));
        }
    }


    private List<GameObject> testObjects = new List<GameObject>();
    public bool CollidesWithBricks(Quaternion rot, Vector3 pos)
    {
        int mask = LayerMask.GetMask("lego", "placed lego");

        // For some reason, the math here is all 180 degrees off. So rotate the object 180 degrees around it's UP axis
        _transform.RotateAround(_transform.position, _transform.up, 180f);

        foreach (BoxCollider c in colliders)
        {
            if (Physics.CheckBox(c.transform.TransformPoint(c.center),
                Vector3.Scale(c.size / 2, c.transform.lossyScale), c.transform.rotation, mask,
                QueryTriggerInteraction.Ignore))
                return true;
        }

        return false;
    }

    private GameObject[] GetFemaleConnectorsWithConnections()
    {
        return _femaleConnectors.Where(c => ClosestConnectorFromConnector(c)).ToArray();
    }

    private GameObject[] GetMaleConnectorsWithConnections()
    {

        return _maleConnectors.Where(c => ClosestConnectorFromConnector(c)).ToArray();
    }

    // Checks if there is a valid way to orient the connections
    private GameObject[] ValidConnections(GameObject[] connectors)
    {
        if (connectors.Length == 0)
        {
            return new GameObject[]{};
        }

        if (connectors.Length == 1)
        {
            return connectors;
        }

        Dictionary<GameObject, (float, GameObject)> partnerConnectorDistanceMap = new Dictionary<GameObject, (float, GameObject)>();
        // First, filter out femaleConnectors that are contending for the same male connector
        foreach (GameObject connector in connectors)
        {
            GameObject partnerConnector = ClosestConnectorFromConnector(connector);
            float distance = DistanceBetweenConnectors(connector, partnerConnector);

            // If the female connector is not in the map yet, it means no female before has contested for this male connector
            if (!partnerConnectorDistanceMap.ContainsKey(partnerConnector))
            {
                partnerConnectorDistanceMap[partnerConnector] = (distance, connector);
            }
            else
            {
                (float otherDistance, GameObject _) = partnerConnectorDistanceMap[partnerConnector];
                if (distance < otherDistance)
                {
                    partnerConnectorDistanceMap[partnerConnector] = (distance, connector);
                }
            }
        }

        List<GameObject> eligibleFemaleConnectors = new List<GameObject>();
        (float, GameObject)[] keys = partnerConnectorDistanceMap.Values.ToArray();
        foreach ((float distance, GameObject femaleConnector) in keys)
        {
            eligibleFemaleConnectors.Add(femaleConnector);
        }

        return eligibleFemaleConnectors.ToArray();
    }

    private Quaternion GetNewRot(GameObject otherBrick)
    {
        Quaternion rot = transform.rotation;
        Quaternion otherRot = otherBrick.transform.rotation;

        float identityDiff = Quaternion.Angle(otherRot, rot);

        Quaternion ninetyRot = otherRot * Quaternion.Euler(Vector3.up * 90);
        float ninetyDiff = Quaternion.Angle(ninetyRot, rot);

        Quaternion oneEightyRot = otherRot * Quaternion.Euler(Vector3.up * 180);
        float oneEightyDiff = Quaternion.Angle(oneEightyRot, rot);

        Quaternion twoSeventyRot = otherRot * Quaternion.Euler(Vector3.up * 270);
        float twoSeventyDiff = Quaternion.Angle(twoSeventyRot, rot);

        float maxDiff = Mathf.Max(identityDiff, ninetyDiff, oneEightyDiff, twoSeventyDiff);
        if (maxDiff == identityDiff)
        {
            return otherBrick.transform.rotation;
        }
        else if (maxDiff == ninetyDiff)
        {
            return ninetyRot;
        }
        else if (maxDiff == oneEightyDiff)
        {
            return oneEightyRot;
        }
        else if (maxDiff == twoSeventyDiff)
        {
            return twoSeventyRot;
        }
        else
        {
            Debug.Log("SOMETHING AWFUL HAS HAPPENED FUUUUCK");
            return Quaternion.identity;
        }
    }

    private Vector3 GetNewPosWithRot(Quaternion rot, GameObject otherBrick, GameObject[] femaleConnectors, bool connectingDownwards)
    {
        Vector3 otherBrickPos = otherBrick.transform.position;
        Quaternion oldRot = transform.rotation;
        _transform.rotation = rot;

        // TODO: Figure out where the heck these constants come from.
        // This stuff is so jank D:
        float heightDelta = connectingDownwards ? Height() - 0.060f : 0.0478f - Height();
        if (window && !connectingDownwards)
        {
            heightDelta += 0.001f;
        }

        Vector3 newPos = _transform.position;
        newPos.y = ClosestConnectorFromConnector(femaleConnectors[0]).transform.position.y + heightDelta;
        newPos.x = otherBrickPos.x
                   + (femaleConnectors[0].transform.position.x - newPos.x + (ClosestConnectorFromConnector(femaleConnectors[0]).transform.position.x - otherBrickPos.x));

        newPos.z = otherBrickPos.z
                   + (femaleConnectors[0].transform.position.z - newPos.z + (ClosestConnectorFromConnector(femaleConnectors[0]).transform.position.z - otherBrickPos.z));

        transform.rotation = oldRot;

        return newPos;
    }

    private static GameObject ClosestConnectorFromConnector(GameObject connector)
    {
        return connector.GetComponent<LegoConnectorScript>().ClosestConnector();
    }

    private static float DistanceBetweenConnectors(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private static bool AngleApproximatelyZero(float a)
    {
        return Mathf.Abs(a % 360f) < 0.5f || Mathf.Abs(a % 360f) > 359.5f;
    }

    private float Height()
    {
        if (heightOverride != 0) return heightOverride;

        return window ? 0.113f : (tile ? 0.0565f : 0.078736f);
    }

    private bool IsVectorApproximatelyOne(Vector3 vec)
    {
        return ApproximatelyEqual(vec.x, 1f) && ApproximatelyEqual(vec.y, 1f) && ApproximatelyEqual(vec.z, 1f);
    }

    private bool ApproximatelyEqual(float value, float target)
    {
        return Mathf.Abs(value - target) < 0.005f;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        // Vector3 extents = model.GetComponent<MeshFilter>().sharedMesh.bounds.extents;

        // TODO: Instead of dividing by 3, we should subtract the height of the studs. This code won't work with plate pieces.
        // collisionExtents = Vector3.Scale(new Vector3(extents.x, extents.y / 4, extents.z), model.transform.localScale);

        _maleConnectors = new List<GameObject>();
        foreach (Transform child in maleConnectorParent.transform)
        {
            _maleConnectors.Add(child.gameObject);
        }

        _femaleConnectors = new List<GameObject>();
        foreach (Transform child in femaleConnectorParent.transform)
        {
            _femaleConnectors.Add(child.gameObject);
        }
    }
}
