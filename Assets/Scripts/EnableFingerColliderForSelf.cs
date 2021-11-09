using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class EnableFingerColliderForSelf : MonoBehaviour
{
    public RealtimeAvatar realtimeAvatar;
    public Collider fingerCollider;

    // Start is called before the first frame update
    private void Start()
    {
        if (!realtimeAvatar.isOwnedLocallyInHierarchy)
        {
            fingerCollider.enabled = false;
        }
    }
}
