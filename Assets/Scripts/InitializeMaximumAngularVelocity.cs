using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeMaximumAngularVelocity : MonoBehaviour
{
    public float initialMaximumAngularVelocity = 7;

    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<Rigidbody>().maxAngularVelocity = initialMaximumAngularVelocity;
    }
}
