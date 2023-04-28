using UnityEngine.UI;
using UnityEngine;

public class RoomGravity : MonoBehaviour
{

    private static Vector3 _lowGravityVector = new Vector3(0, -1f, 0);
    private static Vector3 _regularGravityVector = new Vector3(0, -9.8f, 0);

    public void OnValueChanged(bool value)
    {
        Physics.gravity = value ? _lowGravityVector : _regularGravityVector;
    }
}
