using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBrickLowerTrigger : MonoBehaviour
{
    // When a brick enters the trigger, send it back up to the top
    private void OnTriggerEnter(Collider other)
    {
        Transform brickTransform = other.transform.parent.gameObject.transform;
        Vector3 oldPos = brickTransform.localPosition;
        brickTransform.localPosition = new Vector3(oldPos.x, 40f, oldPos.z);
    }
}
