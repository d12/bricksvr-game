using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PagedScroll : MonoBehaviour
{
    public Button upButton;
    public Button downButton;

    public int itemsPerPage;

    public GameObject listObject;
    public Transform listObjectTransform;

    public List<GameObject> listChildren;

    private int _page;
    private int _cachedChildCount;

    private int fixedElementCount;
    private bool usingFixedElementCount;

    private int LastPage => (_cachedChildCount - 1) / itemsPerPage;

    // private void Update()
    // {
    //     if (usingFixedElementCount) return;
    //
    //     if (listObjectTransform.childCount != _cachedChildCount)
    //         RepopulateListChildren();
    // }

    public void SetFixedElementCount(int count)
    {
        fixedElementCount = count;
        usingFixedElementCount = true;
        RepopulateListChildren();
    }

    public void DisableButtons()
    {
        upButton.interactable = false;
        downButton.interactable = false;
    }

    public void PageDown()
    {
        _page = Mathf.Min(LastPage, _page + 1);
        SetListChildrenVisibilities();

        SetPageButtonStates();
    }

    public void PageUp()
    {
        _page = Mathf.Max(0, _page - 1);
        SetListChildrenVisibilities();

        SetPageButtonStates();
    }

    private void RepopulateListChildren()
    {
        _cachedChildCount = usingFixedElementCount ? fixedElementCount : listObjectTransform.childCount;
        listChildren.Clear();
        int i = 0;
        foreach (Transform objTransform in listObjectTransform)
        {
            if (usingFixedElementCount && i >= fixedElementCount) break;
            listChildren.Add(objTransform.gameObject);
            i++;
        }

        SetListChildrenVisibilities();
        SetPageButtonStates();
    }

    private void SetListChildrenVisibilities()
    {
        int lowerBound = _page * itemsPerPage;
        int upperBound = usingFixedElementCount ? Mathf.Min(((_page + 1) * itemsPerPage) - 1, fixedElementCount - 1) : ((_page + 1) * itemsPerPage) - 1;

        for (int i = 0; i < listObjectTransform.childCount; i++)
        {
            listObjectTransform.GetChild(i).gameObject.SetActive(i >= lowerBound && i <= upperBound);
        }
    }

    private void SetPageButtonStates()
    {
        upButton.interactable = _page != 0;
        downButton.interactable = _page != LastPage;
    }
}
