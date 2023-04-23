using UnityEngine.Android;
using UnityEngine;

public class PromptFileSystem : MonoBehaviour
{
    public void Check() {
        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
    }
}
