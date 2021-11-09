using System;
using UnityEngine;
using Oculus.Platform;

public class OculusEntitlementCheck : BaseEntitlementCheck
{
    private Action<bool, string> _entitlementCallback;

    public override void IsEntitled(Action<bool, string> entitlementManagerCallback)
    {
        _entitlementCallback = entitlementManagerCallback;

        try
        {
            Core.AsyncInitialize();
            Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementCallback);
        }
        catch(Exception e)
        {
            Debug.LogError("Platform failed to initialize due to exception.");
            Debug.LogException(e);

            entitlementManagerCallback.Invoke(false, null);
        }
    }

    private void EntitlementCallback(Message msg)
    {
        if (!msg.IsError)
            Users.GetLoggedInUser().OnComplete(message =>
            {
                _entitlementCallback.Invoke(!msg.IsError, message.Data.ID.ToString());
            });
    }
}