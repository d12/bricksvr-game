using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CreateScriptHelper
{
    private const string MonobehaviorTemplateFile = "Assets/Templates/Monobehavior.template";
    private const string SingletonTemplateFile = "Assets/Templates/Singleton.template";

    [MenuItem("Generator/New Script/Monobehavior")]
    private static void OpenMonobehaviorWindow()
    {
        NewScriptEditor nameWindow = EditorWindow.GetWindow<NewScriptEditor>();
        nameWindow.templateToUse = MonobehaviorTemplateFile;
        nameWindow.scriptTemplateName = "Monobehavior";

        nameWindow.CenterOnMainWin();

        nameWindow.Show();
    }

    // [MenuItem("Generator/New Script/Singleton")]
    // private static void OpenSingletonWindow()
    // {
    //     NewScriptEditor nameWindow = EditorWindow.GetWindow<NewScriptEditor>();
    //     nameWindow.templateToUse = MonobehaviorTemplateFile;
    //     nameWindow.scriptTemplateName = "Singleton";
    //
    //     nameWindow.CenterOnMainWin();
    //
    //     nameWindow.Show();
    // }

    public class NewScriptEditor : EditorWindow
    {
        private string className;

        public string scriptTemplateName;
        public string templateToUse;
        private void OnGUI()
        {
            GUILayout.Label($"Create a new {scriptTemplateName}", EditorStyles.boldLabel);
            className = EditorGUILayout.TextField("Script name", className);
            GUILayout.Space(60);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create")) {
                CreateMonobehavior();
                Close();
            }
            if (GUILayout.Button("Cancel")) Close();
            EditorGUILayout.EndHorizontal();
        }

        private void CreateMonobehavior()
        {
            string filePath = $"Assets/Scripts/{className}.cs";
            if (File.Exists(filePath))
            {
                Debug.LogError($"{filePath} already exists. Exiting!");
                return;
            }

            string fileContents = File.ReadAllText(templateToUse);
            fileContents = fileContents.Replace("CLASSNAME", className);
            File.WriteAllText(filePath, fileContents);

            Debug.Log($"Created monobehavior at {filePath}");
        }
    }
}

public static class Extensions
{
    public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
    {
        var result = new List<System.Type>();
        var assemblies = aAppDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(aType))
                    result.Add(type);
            }
        }
        return result.ToArray();
    }

    public static Rect GetEditorMainWindowPos()
    {
        var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
        if (containerWinType == null)
            throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
        var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (showModeField == null || positionProperty == null)
            throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
        var windows = Resources.FindObjectsOfTypeAll(containerWinType);
        foreach (var win in windows)
        {
            var showmode = (int)showModeField.GetValue(win);
            if (showmode == 4) // main window
            {
                var pos = (Rect)positionProperty.GetValue(win, null);
                return pos;
            }
        }
        throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
    }

    public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin)
    {
        var main = GetEditorMainWindowPos();
        var pos = aWin.position;
        float w = (main.width - pos.width)*0.5f;
        float h = (main.height - pos.height)*0.5f;
        pos.x = main.x + w;
        pos.y = main.y + h;
        aWin.position = pos;
    }
}