using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeNewMenu : MonoBehaviour
{
    public GameObject mainMenu;

    public GameObject optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
