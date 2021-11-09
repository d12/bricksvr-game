using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Analytics;

public class EntitlementCheckManager : MonoBehaviour
{
    public enum EntitlementPlatforms
    {
        None,
        OculusQuest,
        Steam,
    }

    [SerializeField]
    public EntitlementPlatforms entitlementPlatform;

    public string userId;

    private bool _isAwaitingEntitlementResults;
    private float _timeEntitlementStarted;

    private const float EntitlementTimeout = 8f;
    private const bool KillAppOnEntitlementFailure = true;

    private static EntitlementCheckManager _instance;

    public static EntitlementCheckManager GetInstance()
    {
        if (_instance == null)
            _instance = GameObject.Find("MetaObjects/EntitlementChecker").GetComponent<EntitlementCheckManager>();

        return _instance;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        _isAwaitingEntitlementResults = true;
        _timeEntitlementStarted = Time.time;

        BaseEntitlementCheck check = EntitlementCheckToUse();

        if (check != null)
        {
            Debug.Log($"Beginning entitlement check using entitlement provider {entitlementPlatform.ToString()}");
            check.IsEntitled(EntitlementCallback);
        }
        else
        {
            // If no entitlement check is returned, something is configured wrong. Default to entitlement failure.
            EntitlementCallback(false, null);
        }
    }

    // If we haven't received an entitlement result within EntitlementTimeout seconds, fail the entitlement.
    private void Update()
    {
        if (_isAwaitingEntitlementResults && (_timeEntitlementStarted + EntitlementTimeout < Time.time))
        {
            Debug.LogError("Entitlement timed out.");
            EntitlementCallback(false, null);
        }
    }

    private void EntitlementCallback(bool isEntitled, string entitlementUserId)
    {
        if (!_isAwaitingEntitlementResults)
        {
            Debug.LogError("Received more than one entitlement result. This may mean something's wrong.");
        }

        Analytics.CustomEvent("entitlement", new Dictionary<string, object>()
        {
            { "success", isEntitled},
            { "entitlement-provider", entitlementPlatform.ToString()},
            { "user-id-obtained", !string.IsNullOrEmpty(entitlementUserId)},
            { "platform", OVRPlugin.GetSystemHeadsetType().ToString()}
        });

        _isAwaitingEntitlementResults = false;
        if (isEntitled)
        {
            Debug.Log("User entitlement check passed");
            Debug.Log($"User ID is {entitlementUserId}");

            // Disable gameobject, mainly to kill update loops.
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("User entitlement check FAILED");
            if(KillAppOnEntitlementFailure)
                QuitApp();
        }
    }

    private BaseEntitlementCheck EntitlementCheckToUse()
    {
        switch (entitlementPlatform)
        {
            case EntitlementPlatforms.None:
                return GetComponent<NoneEntitlementCheck>();

            case EntitlementPlatforms.OculusQuest:
                return GetComponent<OculusEntitlementCheck>();

            case EntitlementPlatforms.Steam:
                return GetComponent<SteamEntitlementCheck>();

            default:
                return null;
        }
    }

    private static void QuitApp()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
