using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsTabSystem : MonoBehaviour
{
    public Tab[] tabs;
    public int defaultTab;
    private int _selectedTab;

    // Start is called before the first frame update
    void Start()
    {
        _selectedTab = defaultTab;
        SelectTab(_selectedTab);
    }

    public void SelectTab(int index)
    {
        DisableTab(_selectedTab);
        EnableTab(index);

        _selectedTab = index;
    }

    private void DisableTab(int index)
    {
        Tab tab = tabs[index];
        tab.tabObject.SetActive(false);
        tab.tabButtonEvents.SetSelected(false);
    }

    private void EnableTab(int index)
    {
        Tab tab = tabs[index];
        tab.tabObject.SetActive(true);
        tab.tabButtonEvents.SetSelected(true);
    }

    [Serializable]
    public class Tab
    {
        public GameObject tabObject;
        public UIButtonEvents tabButtonEvents;

    }
}
