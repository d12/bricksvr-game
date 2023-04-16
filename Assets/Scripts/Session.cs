using UnityEngine;
using System;

public class Session : MonoBehaviour
{
    public string name = "My world";

    public int clientID {
        get {
            return -1;
        }
    }

    public bool canPlace {
        get {
            if(isMultiPlayer) {
                return false;
            }
            else return isSinglePlayer;
        }
    }

    private bool playing = true;
    public bool isPlaying {
        get {
            return playing;
        }
    }

    public bool isLoading {
        get {
            return false;
        }
    }

    public bool isSinglePlayer {
        get {
            return true;
        }
    }

    public bool isMultiPlayer {
        get {
            return false;
        }
    }

    public event Action<Session> didSessionStart;
    public event Action<Session> didSessionEnd;

    public void Connect(string ip) {
        throw new NotImplementedException();
    }

    public void Start() {

    }
}
