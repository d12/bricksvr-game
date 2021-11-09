using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Normal.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadBricksOnLoad : MonoBehaviour
{
    private Realtime _realtime;
    private string _roomDataJson;
    private TextMeshProUGUI _joiningStatusText;

    public bool upstreamError;
    public bool connectivityError;

    public bool Errored => upstreamError || connectivityError;

    public bool isDoneDownloading;

    private int _downloadSpeedMultiplier = 1;

    public ChunkedRenderer chunkedRenderer;

    public List<NormcoreRPC.Brick> bricksParentedToHeads = new List<NormcoreRPC.Brick>();

    void Start()
    {
        _realtime = GetComponent<Realtime>();
        // if (Application.platform != RuntimePlatform.Android)
        //     _downloadSpeedMultiplier = 3;

        if (Application.isEditor)
        {
            _downloadSpeedMultiplier = 50;
        }
    }

    public void Reset()
    {
        isDoneDownloading = false;
        upstreamError = false;
        connectivityError = false;
    }

    public void StartLoading(string roomName, TextMeshProUGUI joiningStatusText)
    {
        _joiningStatusText = joiningStatusText;
        StartCoroutine(LoadBrickDataAsync(roomName));
    }

    private IEnumerator LoadBrickDataAsync(string roomName)
    {
        _joiningStatusText.text = $"Status: Fetching bricks from database...";
        yield return DownloadBrickData(roomName);

        if (Errored)
        {
            isDoneDownloading = true;
            yield break;
        }

        FetchAllBricksResponse parsedResponse = JsonUtility.FromJson<FetchAllBricksResponse>(_roomDataJson);
        BrickAttach[] createdBricks = new BrickAttach[parsedResponse.bricks.Length];

        chunkedRenderer.enabled = false;

        for (int i = 0; i < parsedResponse.bricks.Length; i++)
        {
            try
            {
                if (parsedResponse.bricks[i].usingHeadStuff && (parsedResponse.bricks[i].headClientId != -1))
                {
                    bricksParentedToHeads.Add(parsedResponse.bricks[i]);
                }
                else
                {
                    createdBricks[i] = PlacedBrickCreator.CreateFromBrickObject(parsedResponse.bricks[i], false)
                        .GetComponent<BrickAttach>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed To load a brick {parsedResponse.bricks[i].type} - {parsedResponse.bricks[i].uuid}");
                Debug.LogException(e);
            }

            if ((createdBricks[i] != null) && (i % (8 * _downloadSpeedMultiplier)) == 0)
            {
                yield return null;
                _joiningStatusText.text = $"Status: Loaded {i}/{parsedResponse.bricks.Length} bricks...";
            }
        }

        for (int i = 0; i < parsedResponse.bricks.Length; i++)
        {
            if (createdBricks[i] == null)
                continue;

            createdBricks[i].RecalculateEnabledConnectors();
            createdBricks[i].RecalculateRenderedGeometry();

            if (i % (16 * _downloadSpeedMultiplier) == 0)
            {
                yield return null;
                _joiningStatusText.text = $"Status: Optimized {i}/{parsedResponse.bricks.Length} bricks...";
            }
        }

        chunkedRenderer.enabled = true;

        GameObject.FindWithTag("RoomOwnershipSync").GetComponent<RoomOwnershipSync>()
            .SetOwnerIdPrefix(parsedResponse.ownerShortIdPrefix);

        // TODO: Normcore sometimes gets out of sync with the Firebase locked state. Make them sync here.

        isDoneDownloading = true;
    }

    public void LoadBricksParentedToHeads()
    {
        if (bricksParentedToHeads.Count > 1000)
        {
            Debug.LogError("Found an absurd number of bricks to parent, something is wrong");
            return;
        }

        int clientId = _realtime.clientID;

        BrickAttach[] createdBricks = new BrickAttach[bricksParentedToHeads.Count];
        RealtimeAvatarManager avatarManager = LookupRealtimeAvatarManager.GetInstance();
        for(int i = 0; i < bricksParentedToHeads.Count; i++)
        {
            if ((bricksParentedToHeads[i].headClientId == clientId) || !avatarManager.avatars.ContainsKey(bricksParentedToHeads[i].headClientId))
            {
                // Users should never have bricks on their head on load
                BrickServerInterface.GetInstance().RemoveBrick(bricksParentedToHeads[i].uuid, _realtime);
            }
            else
            {
                createdBricks[i] = PlacedBrickCreator.CreateFromBrickObject(bricksParentedToHeads[i], false)
                 .GetComponent<BrickAttach>();
            }
        }

        for (int i = 0; i < createdBricks.Length; i++)
        {
            if (createdBricks[i] == null)
                continue;

            createdBricks[i].RecalculateEnabledConnectors();
            createdBricks[i].RecalculateRenderedGeometry();
        }

        bricksParentedToHeads.Clear();
    }

    private IEnumerator DownloadBrickData(string room)
    {
        UnityWebRequest request = UnityWebRequest.Get($"https://us-central1-bricksvr-unity.cloudfunctions.net/fetch-bricks?room={room}&userid={UserId.Get()}");
        request.timeout = 45;

        yield return request.SendWebRequest();

        connectivityError = request.isNetworkError;
        upstreamError = request.isHttpError;

        _roomDataJson = request.downloadHandler.text;
    }

    [Serializable]
    public class FetchAllBricksResponse
    {
        public NormcoreRPC.Brick[] bricks;
        public string ownerShortIdPrefix;
    }
}
