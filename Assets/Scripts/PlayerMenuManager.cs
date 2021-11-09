using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Normal.Realtime;
using UnityEngine;

public class PlayerMenuManager : MonoBehaviour
{
    public RealtimeAvatarManager avatarManager;
    public GameObject playerEntryPrefab;
    public RoomOwnershipSync ownershipSync;

    public List<RealtimeAvatar> users = new List<RealtimeAvatar>();

    public GameObject listParent;
    private Transform _listParentTransform;

    // Start is called before the first frame update
    void Awake()
    {
        _listParentTransform = listParent.transform;
    }

    private void OnEnable()
    {
        avatarManager.avatarCreated += UserJoined;
        avatarManager.avatarDestroyed += UserQuit;

        RefreshPlayerList();
    }

    private void OnDisable()
    {
        avatarManager.avatarCreated -= UserJoined;
        avatarManager.avatarDestroyed -= UserQuit;
    }

    private void RefreshPlayerList()
    {
        RepopulateUserList();
        RebuildUI();
    }

    private void RepopulateUserList()
    {
        users = avatarManager.avatars.Values.OrderBy(avatar => !avatar.isOwnedLocallyInHierarchy).ToList();
    }

    private void RebuildUI()
    {
        foreach (Transform t in _listParentTransform)
            Destroy(t.gameObject);

        foreach (RealtimeAvatar avatar in users)
        {
            GameObject newPlayerEntry = Instantiate(playerEntryPrefab, _listParentTransform);
            PlayerListItem playerListItem = newPlayerEntry.GetComponent<PlayerListItem>();
            playerListItem.Initialize(avatar, ownershipSync);
        }
    }

    private void UserJoined(RealtimeAvatarManager _, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        RefreshPlayerList();
    }

    private void UserQuit(RealtimeAvatarManager _, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        RefreshPlayerList();
    }
}
