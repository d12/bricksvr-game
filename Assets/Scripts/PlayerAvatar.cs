using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    public bool isLocal {
        get {
            return this == AvatarManager.GetInstance().localAvatar;
        }
    }

    private Transform _head;
    public Transform head {
        get {
            return _head;
        }
    }

    private Transform _leftHand;
    public Transform leftHand {
        get {
            return _leftHand;
        }
    }

    private Transform _rightHand;
    public Transform rightHand {
        get {
            return _rightHand;
        }
    }
}
