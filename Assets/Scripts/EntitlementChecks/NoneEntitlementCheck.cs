using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneEntitlementCheck : BaseEntitlementCheck
{
    public override void IsEntitled(Action<bool, string> entitlementManagerCallback)
    {
        entitlementManagerCallback.Invoke(true, null);
    }
}
