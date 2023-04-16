using UnityEngine;

public class DestroyFallingBricks : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Lego"))
        {
            BrickAttach attach = otherObject.GetComponentInParent<BrickAttach>();
            if (attach == null) return;

            GameObject brick = attach.gameObject;

            BrickStore.GetInstance().Delete(attach.GetUuid());
            BrickDestroyer.GetInstance().DelayedDestroy(brick);
        }
    }
}