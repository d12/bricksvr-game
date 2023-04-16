using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine;

public class BrickDestroyer : MonoBehaviour
{
    private static BrickDestroyer _instance;

    public static BrickDestroyer GetInstance()
    {
        if (!_instance) _instance = FindObjectOfType<BrickDestroyer>();
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public void DelayedDestroy(GameObject obj)
    {
        StartCoroutine(DelayedDestroyIEnum(obj));
    }

    private IEnumerator DelayedDestroyIEnum(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        // string uuid = obj.GetComponent<BrickAttach>().GetUuid();
        // BrickStore.GetInstance().Delete(uuid);

        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        XRGrabInteractable _interactable = obj.GetComponent<XRGrabInteractable>();
        if (_interactable != null) _interactable.interactionLayerMask = 0;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        PlacedBrickCreator.DestroyBrickObject(obj);

        yield return null;
        yield return null;

        BrickMeshRecalculator.GetInstance().RecalculateNearbyBricks(pos);

        yield return null;
    }


}
