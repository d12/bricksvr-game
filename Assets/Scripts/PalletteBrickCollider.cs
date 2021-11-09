using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletteBrickCollider : MonoBehaviour
{
    public BrickPickerManager brickPickerManager;
    public int collisionIndex;

    private void OnTriggerEnter(Collider c)
    {
        BrickAttach attach = c.gameObject.GetComponentInParent<BrickAttach>();
        if (!attach) return;
        if (!attach.Held) return;

        collisionIndex += 1;

        brickPickerManager.SetColor(attach.Color);
        brickPickerManager.SetSliders(attach.Color);
    }
}
