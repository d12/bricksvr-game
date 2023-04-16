using UnityEngine;

public class SetClientIdOnHeadBricks : MonoBehaviour
{
    // Add multiplayer support later.
    void Start()
    {
        int clientId = -1;

        BrickAttach[] attaches = GetComponentsInChildren<BrickAttach>();
        foreach (BrickAttach attach in attaches)
            attach.headClientId = clientId;
    }
}
