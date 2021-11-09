using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BrickServerInterface : MonoBehaviour
{
    public RoomOwnershipSync roomOwnershipSync;
    private static BrickServerInterface _instance;
    private int _failedBricks = 0;

    // String keys for web reqeusts, allocate these ahead of time so we don't have slow first web requests
    private const string BrickServerInterfaceTag = "SendBrickToServer";
    private const string EditorExceptionSuffix = "-editor";
    private const string NullString = "null";
    private const string RoomKey = "room";
    private const string RoomNameKey = "name";
    private const string UserIdKey = "userid";
    private const string VersionKey = "version";
    private const string MinorVersionKey = "minorversion";
    private const string ConditionKey = "condition";
    private const string StacktraceKey = "stacktrace";
    private const string TypeKey = "type";
    private const string MatIdKey = "matId";
    private const string ColorKey = "color";
    private const string BrickIdKey = "brickid";
    private const string LockedKey = "locked";
    private const string NicknameKey = "nickname";
    private const string HeadClientId = "headClientId";

    private const string PosXKey = "posx";
    private const string PosYKey = "posy";
    private const string PosZKey = "posz";
    private const string RotXKey = "rotx";
    private const string RotYKey = "roty";
    private const string RotZKey = "rotz";
    private const string RotWKey = "rotw";

    private const string TimestampKey = "timestamp";

    private const string Done = "done";

    public static BrickServerInterface GetInstance()
    {
        if (_instance == null) _instance = GameObject.FindWithTag(BrickServerInterfaceTag).GetComponent<BrickServerInterface>();

        return _instance;
    }

    public void SendBrick(NormcoreRPC.Brick brick, Realtime realtime)
    {
        _instance.StartCoroutine(SendBrickIEnum(brick, realtime));
    }

    public void RemoveBrick(string uuid, Realtime realtime)
    {
        _instance.StartCoroutine(RemoveBrickIEnum(uuid, realtime));
    }

    public void SetLocked(bool locked, Realtime realtime)
    {
        _instance.StartCoroutine(SetLockedIEnum(locked, realtime));
    }

    public void SetNickname(string nickname)
    {
        _instance.StartCoroutine(SetNicknameIEnum(nickname));
    }

    public IEnumerator SendException(string condition, string stacktrace, LogType type)
    {
        WWWForm form = new WWWForm();
        Realtime realtime = Realtime.instances.First();

        string version = ReleaseVersion.VersionString();
        if (Application.isEditor) version += EditorExceptionSuffix;

        form.AddField(RoomKey, realtime.room == null ? NullString : realtime.room.name);
        form.AddField(UserIdKey, UserId.Get());
        form.AddField(VersionKey, version);
        form.AddField(ConditionKey, condition);
        form.AddField(StacktraceKey, stacktrace);
        form.AddField(TypeKey, type.ToString());

        UnityWebRequest request = UnityWebRequest.Post(ExceptionURL, form);
        request.timeout = 20;

        yield return request.SendWebRequest();

        // Debug.Log("Reported exception. Response:");
        // Debug.Log(request.downloadHandler.text);
    }

    private IEnumerator SendBrickIEnum(NormcoreRPC.Brick brick, Realtime realtime)
    {
        WWWForm form = new WWWForm();

        yield return null; // Wait a frame, the frame where we attach a brick is already a busy frame.

        form.AddField(PosXKey, brick.pos.x.ToString(CultureInfo.InvariantCulture));
        form.AddField(PosYKey, brick.pos.y.ToString(CultureInfo.InvariantCulture));
        form.AddField(PosZKey, brick.pos.z.ToString(CultureInfo.InvariantCulture));

        form.AddField(RotXKey, brick.rot.x.ToString(CultureInfo.InvariantCulture));
        form.AddField(RotYKey, brick.rot.y.ToString(CultureInfo.InvariantCulture));
        form.AddField(RotZKey, brick.rot.z.ToString(CultureInfo.InvariantCulture));
        form.AddField(RotWKey, brick.rot.w.ToString(CultureInfo.InvariantCulture));

        form.AddField(TypeKey, brick.type);
        form.AddField(MatIdKey, brick.matId);
        form.AddField(ColorKey, brick.color);
        form.AddField(RoomKey, realtime.room.name);
        form.AddField(BrickIdKey, brick.uuid);
        form.AddField(UserIdKey, UserId.Get());
        form.AddField(HeadClientId, brick.headClientId);
        form.AddField(TimestampKey, realtime.room.time.ToString(CultureInfo.InvariantCulture));

        UnityWebRequest request = UnityWebRequest.Post(BrickSubmitURL, form);
        yield return request.SendWebRequest();

        string response = request.downloadHandler.text;
        if (response == Done)
        {
            // Debug.Log("Successfully submitted brick");
        }
        else
        {
            Debug.LogError($"ERROR submitting brick! {response}");
            if (response.Contains("permissions"))
            {
                Debug.LogError("Room is locked, setting lock in realtime db");
                roomOwnershipSync.SetLocked(true);
            }
            _failedBricks += 1;

            if (_failedBricks > 5)
            {
                Debug.LogError("TOO MANY FAILED BRICKS, Heading back to main screen");
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }
        }
    }

    private IEnumerator RemoveBrickIEnum(string uuid, Realtime realtime)
    {
        WWWForm form = new WWWForm();

        form.AddField(BrickIdKey, uuid);
        form.AddField(RoomKey, realtime.room.name);
        form.AddField(UserIdKey, UserId.Get());
        form.AddField(TimestampKey, realtime.room.time.ToString(CultureInfo.InvariantCulture));

        UnityWebRequest request = UnityWebRequest.Post(RemoveBricksURL, form);
        yield return request.SendWebRequest();

        string response = request.downloadHandler.text;
        if (response == Done)
        {
            // Debug.Log("Successfully removed brick");
        }
        else
        {
            if (response.Contains("permissions"))
            {
                Debug.LogError("Room is locked, setting lock in realtime db");
                roomOwnershipSync.SetLocked(true);
            }

            if (response.Contains("outdated"))
            {
                // Debug.Log("Outdated delete");
                yield break;
            }

            _failedBricks += 1;

            if (_failedBricks > 5)
            {
                Debug.LogError("TOO MANY FAILED BRICKS, Heading back to main screen");
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }

            Debug.LogError($"ERROR removing brick! {response}");
        }
    }

    private IEnumerator SetLockedIEnum(bool locked, Realtime realtime)
    {
        WWWForm form = new WWWForm();
        form.AddField(RoomKey, realtime.room.name);
        form.AddField(UserIdKey, UserId.Get());
        form.AddField(LockedKey, locked.ToString());

        UnityWebRequest request = UnityWebRequest.Post(SetLockedURL, form);
        yield return request.SendWebRequest();

        string response = request.downloadHandler.text;
        if (response == Done)
        {
            // Debug.Log($"Successfuly set room permission to {locked}");
        }
        else
        {
            Debug.LogError($"ERROR setting room permission! {response}");
        }
    }

    public IEnumerator CreateRoom(string roomName)
    {
        WWWForm form = new WWWForm();
        form.AddField(RoomNameKey, roomName);
        form.AddField(UserIdKey, UserId.Get());
        form.AddField(MinorVersionKey, ReleaseVersion.MinorString());

        UnityWebRequest request = UnityWebRequest.Post(CreateRoomURL, form);
        request.timeout = 15;

        yield return request.SendWebRequest();


        CreateRoomResponse response = JsonUtility.FromJson<CreateRoomResponse>(request.downloadHandler.text);
        if (response == null)
        {
            response = new CreateRoomResponse { error = "An unknown error has occured" };
        }
        response.connectivityError = request.isNetworkError;
        response.upstreamError = request.isHttpError;

        yield return response;
    }

    public IEnumerator StartExport(string roomName)
    {
        WWWForm form = new WWWForm();
        form.AddField(RoomKey, roomName);

        UnityWebRequest request = UnityWebRequest.Post(StartExportURL, form);
        request.timeout = 15;

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        StartExportResponse responseObject = new StartExportResponse();

        switch (responseText)
        {
            case "Success":
                responseObject.Success = true;
                break;
            case "Error: Room was exported less than 5 minutes ago":
                responseObject.RateLimited = true;
                break;
            case "Error: Room has no bricks":
                responseObject.EmptyRoom = true;
                break;

        }

        yield return responseObject;
    }

    public IEnumerator RoomInfo(string normcoreRoomName)
    {
        WWWForm form = new WWWForm();
        form.AddField(RoomKey, normcoreRoomName);

        UnityWebRequest request = UnityWebRequest.Post(RoomInfoURL, form);
        request.timeout = 15;

        yield return request.SendWebRequest();

        RoomInfoResponse response = JsonUtility.FromJson<RoomInfoResponse>(request.downloadHandler.text) ??
                                    new RoomInfoResponse();

        response.error = request.isHttpError || request.isNetworkError;

        yield return response;
    }

    private IEnumerator SetNicknameIEnum(string nickname)
    {
        WWWForm form = new WWWForm();
        form.AddField(UserIdKey, UserId.Get());
        form.AddField(NicknameKey, nickname);

        UnityWebRequest request = UnityWebRequest.Post(SetNicknameURL, form);
        request.timeout = 15;

        yield return request.SendWebRequest();

        string response = request.downloadHandler.text;
        if (response != Done)
        {
            Debug.LogError($"ERROR setting nickname: {response}");
        }
    }

    public IEnumerator GetIsVersionSupported()
    {
        WWWForm form = new WWWForm();

        form.AddField(VersionKey, ReleaseVersion.VersionString());

        UnityWebRequest request = UnityWebRequest.Post(IsVersionSupportedURL, form);
        request.timeout = 10;

        yield return request.SendWebRequest();

        string response = request.downloadHandler.text;

        yield return JsonUtility.FromJson<IsVersionSupportedResponse>(request.downloadHandler.text);
    }
}

[Serializable]
public class CreateRoomResponse
{
    public string name;
    public string code;
    public string normcoreRoom;
    public string error;
    public bool connectivityError;
    public bool upstreamError;
}

public struct StartExportResponse
{
    public bool Success;
    public bool RateLimited;
    public bool EmptyRoom;
}

[Serializable]
public class RoomInfoResponse
{
    public string ownerIdPrefix;
    public string brickCount;
    public string locked;
    public string name;
    public bool error;
    public bool Exists => name != null;
}

[Serializable]
public class NicknameResponse
{
    public string nickname;
}

[Serializable]
public class IsVersionSupportedResponse
{
    public bool supported;
}
