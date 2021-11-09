using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateUserIdText : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = SystemInfo.deviceUniqueIdentifier.Substring(0, 8).ToLower();
    }
}