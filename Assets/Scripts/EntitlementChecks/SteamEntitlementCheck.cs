using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamEntitlementCheck : BaseEntitlementCheck
{
    public SteamManager steamManager;
    private bool _isCheckingEntitlement;

    public override void IsEntitled(Action<bool, string> entitlementCallback)
    {
        bool initializationSucceeded = steamManager.Initialize();
        if (!initializationSucceeded)
        {
            entitlementCallback.Invoke(false, null);
            return;
        }

        // steamManager.RestartAppIfNecessary();

        entitlementCallback.Invoke(true, steamManager.GetUserId().ToString());
    }
}