using UnityEngine;

public class EnableFingerColliderForSelf : MonoBehaviour
{
    public PlayerAvatar realtimeAvatar;
    public Collider fingerCollider;

    private void Start()
    {
        if (!realtimeAvatar.isLocal)
        {
            fingerCollider.enabled = false;
        }
    }
}
