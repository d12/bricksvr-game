using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExportManager : MonoBehaviour
{
    public Button backButton;

    public Button exportButton;

    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;
    public TextMeshProUGUI urlText;
    public Button urlButton;

    public SessionManager SessionManager;

    private const string InitialTopText = "Exported files include all bricks and materials. You can open these files in Blender or other programs.";

    private const string InitialBottomText =
        "Click Export to start an export. The export will happen in the background.";

    private string downloadUrl = "";

    private void OnEnable()
    {
        topText.text = InitialTopText;
        bottomText.text = InitialBottomText;
        urlText.text = "";
        urlButton.enabled = false;
        backButton.interactable = true;
        exportButton.interactable = true;
    }

    public void StartExport()
    {
        // backButton.interactable = false;
        exportButton.interactable = false;
        topText.text = "Starting export...";
        bottomText.text = "";

        StartCoroutine(StartExportCoroutine());
    }

    public IEnumerator StartExportCoroutine()
    {
        CoroutineWithData cd = new CoroutineWithData(this, BrickServerInterface.GetInstance().StartExport(SessionManager.GetRoomName()));
        yield return cd.coroutine;

        StartExportResponse response = (StartExportResponse) cd.result;

        if (response.RateLimited)
        {
            topText.text = "Error: Please wait at least 5 minutes between exports.";
        } else if(response.EmptyRoom)
        {
            topText.text = "Error: You cannot export an empty room!";
        } else if (!response.Success)
        {
            topText.text = "Error: An error occured while starting your export. Please try again later.";
        }
        else
        {
            downloadUrl = $"https://bricksvr.com/exports/{SessionManager.GetRoomName()}";
            topText.text = $"Your export has been started!\n\nYou can download the files at:";
            urlText.text = downloadUrl;
            urlButton.enabled = true;
        }

        exportButton.interactable = !response.Success;
    }

    public void URLClicked()
    {
        Application.OpenURL(downloadUrl);
    }
}
