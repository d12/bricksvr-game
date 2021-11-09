using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBrick : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Add spin.
        // https://www.youtube.com/watch?v=l4mTNQLsD0c
        rb.AddTorque(new Vector3(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f)), ForceMode.Force);
    }
}
