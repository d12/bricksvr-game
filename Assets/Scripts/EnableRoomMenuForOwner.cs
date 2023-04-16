using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnableRoomMenuForOwner : MonoBehaviour
{
    public Toggle lockRoomToggle;
    public Toggle lowGravityToggle;
    public Toggle movingThroughBricksAllowedToggle;
    public TextMeshProUGUI menuSubtitle;

    private const string OwnerText = "You are the room owner, so you can make changes.";
    private const string NotOwnerText = "Only the room owner can change these settings.";

    void OnEnable()
    {
        try
        {
            if (gameObject.activeSelf)
            {
                // TODO: Multiplayer support.
                bool isOwner = true; //ownershipSync.IsRoomOwner();

                // TODO: Remember to make a room config for single-player games.
                lowGravityToggle.interactable = isOwner;
                lowGravityToggle.SetIsOnWithoutNotify(true);

                movingThroughBricksAllowedToggle.interactable = isOwner;
                movingThroughBricksAllowedToggle.SetIsOnWithoutNotify(false);

                menuSubtitle.text = isOwner ? OwnerText : NotOwnerText;
            }
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Tried to check room ownership before we were connected to a room. Did you accidentally leave the room menu enabled while working on it @Nat >:)");
        }
    }
}
