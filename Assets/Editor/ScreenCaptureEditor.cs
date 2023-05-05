using System.IO;
using UnityEditor;
using UnityEngine;

//Credit: https://gist.github.com/Tymski/63da742d595eaf36e80ffe5a36ce30d9
public class ScreenCaptureEditor : EditorWindow
{
    private static string directory = "Screenshots/Capture/";
    private static string latestScreenshotPath = "";
    private bool initDone = false;

    private GUIStyle BigText;

    void InitStyles()
    {
        initDone = true;
        BigText = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
    }

    private void OnGUI()
    {
        if (!initDone)
        {
            InitStyles();
        }

        GUILayout.Label("Screen Capture", BigText);
        if (GUILayout.Button("Take a screenshot"))
        {
            TakeScreenshot();
        }
        GUILayout.Label("Resolution: " + GetResolution());

        if (GUILayout.Button("Reveal in Explorer"))
        {
            ShowFolder();
        }
        GUILayout.Label("Directory: " + directory);
    }

    [MenuItem("Tools/Screenshots/Open Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ScreenCaptureEditor));
    }

    [MenuItem("Tools/Screenshots/Reveal in Explorer")]
    private static void ShowFolder()
    {
        if (File.Exists(latestScreenshotPath))
        {
            EditorUtility.RevealInFinder(latestScreenshotPath);
            return;
        }
        Directory.CreateDirectory(directory);
        EditorUtility.RevealInFinder(directory);
    }

    [MenuItem("Tools/Screenshots/Take a Screenshot")]
    private static void TakeScreenshot()
    {
        Directory.CreateDirectory(directory);
        var currentTime = System.DateTime.Now;
        var filename = currentTime.ToString().Replace('/', '-').Replace(':', '_') + ".png";
        var path = directory + filename;
        ScreenCapture.CaptureScreenshot(path);
        latestScreenshotPath = path;
        Debug.Log($"Screenshot saved: <b>{path}</b> with resolution <b>{GetResolution()}</b>");
    }

    private static string GetResolution()
    {
        Vector2 size = UnityEditor.Handles.GetMainGameViewSize();
        Vector2Int sizeInt = new Vector2Int((int)size.x, (int)size.y);
        return $"{sizeInt.x.ToString()}x{sizeInt.y.ToString()}";
    }

}