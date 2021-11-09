using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntitlementCheck : MonoBehaviour
{
    public virtual void IsEntitled(Action<bool, string> entitlementCallback)
    {
        entitlementCallback.Invoke(false, null);
    }
}
