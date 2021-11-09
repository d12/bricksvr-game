using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuickInteractable : MonoBehaviour
{
    public virtual void Interact(QuickInteractor interactor) { }
}
