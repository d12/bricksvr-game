using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class SetClientIdOnHeadBricks : MonoBehaviour
{
    public RealtimeView rtView;
    // Start is called before the first frame update
    void Start()
    {
        int clientId = rtView.ownerIDInHierarchy;

        BrickAttach[] attaches = GetComponentsInChildren<BrickAttach>();
        foreach (BrickAttach attach in attaches)
            attach.headClientId = clientId;
    }
}
