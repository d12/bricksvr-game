using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BrickStore : MonoBehaviour
{
    private static BrickStore _instance;

    // Maps from UUIDs to brick gameobjects
    private Dictionary<string, GameObject> _dict;
    [FormerlySerializedAs("_tableText")] public TextMeshProUGUI brickCountText;
    private bool _debug = false;

    private void Start()
    {
        _dict = new Dictionary<string, GameObject>();
    }

    public void Put(string key, GameObject brick)
    {
        if(_debug) Debug.Log("Putting " + key);
        _dict[key] = brick;

        if(_debug) Debug.Log("Dict size: " + _dict.Count);

        UpdateTableText();
    }

    public GameObject Get(string key)
    {
        try
        {
            return _dict[key];
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void ClearAndRemoveFromWorld()
    {
        foreach (GameObject brick in _dict.Values)
        {
            if (brick == null) continue;
            brick.GetComponent<BrickAttach>()?.DelayedDestroy();
        }

        _dict.Clear();
        UpdateTableText();
    }

    public void Delete(string key)
    {
        if(_debug) UnityEngine.Debug.Log("Deleting " + key);
        _dict.Remove(key);
        UpdateTableText();
    }

    public Dictionary<string, GameObject>.ValueCollection Values()
    {
        return _dict.Values;
    }

    public Dictionary<string, GameObject>.KeyCollection Keys()
    {
        return _dict.Keys;
    }

    private void UpdateTableText()
    {
        brickCountText.text = _dict.Count.ToString();
    }

    public static BrickStore GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<BrickStore>();
        }

        return _instance;
    }

}

