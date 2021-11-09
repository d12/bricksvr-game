using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFloorConnectorsUnderHand : MonoBehaviour
{
    public GameObject hand;
    private float _connectorInterval = 0.0560751f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = hand.transform.position;
        newPos.y = 0f;
        newPos.x -= newPos.x % _connectorInterval;
        newPos.z -= newPos.z % _connectorInterval;

        transform.position = newPos;
    }
}
