using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMenuManager : MonoBehaviour
{
    public AvatarManager avatarManager;
    public GameObject playerEntryPrefab;

    public List<PlayerAvatar> users = new List<PlayerAvatar>();

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
        users = avatarManager.avatars.Values.OrderBy(avatar => !avatar.isLocal).ToList();
    }

    private void RebuildUI()
    {
        foreach (Transform t in _listParentTransform)
            Destroy(t.gameObject);

        foreach (PlayerAvatar avatar in users)
        {
            GameObject newPlayerEntry = Instantiate(playerEntryPrefab, _listParentTransform);
            PlayerListItem playerListItem = newPlayerEntry.GetComponent<PlayerListItem>();
            playerListItem.Initialize(avatar);
        }
    }

    private void UserJoined(PlayerAvatar avatar)
    {
        RefreshPlayerList();
    }

    private void UserQuit(PlayerAvatar avatar)
    {
        RefreshPlayerList();
    }
}
