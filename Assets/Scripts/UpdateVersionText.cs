using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateVersionText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "v" + ReleaseVersion.VersionString();
    }
}
