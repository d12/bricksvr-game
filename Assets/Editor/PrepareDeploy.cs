using System.IO;
using System.Security.Principal;
using UnityEditor;
using UnityEngine;

public static class PrepareDeploy
{
    private const string MainGamePackageName = "com.BricksVR.BricksVR";
    private const string DemoGamePackageName = "com.BricksVR.BricksVRDemo";

    [MenuItem("Deploy/Increment Version")]
    private static void IncrementVersion()
    {
        string releaseVersionText = File.ReadAllText("Assets/Scripts/ReleaseVersion.cs");
        int patchNumber = ReleaseVersion.Patch;
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("Patch = (\\d+)") ;
        string newReleaseVersionText = regex.Replace(releaseVersionText, $"Patch = {patchNumber + 1}");
        File.WriteAllText("Assets/Scripts/ReleaseVersion.cs", newReleaseVersionText);

        PlayerSettings.Android.bundleVersionCode += 1;

        Debug.Log($"Bumped patch version to {patchNumber + 1}. Make sure to enable the new version server-side. Also update version number.");
    }

    [MenuItem("Deploy/Prepare Windows build")]
    private static void PrepareWindowsBuild()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, MainGamePackageName);

        Debug.Log("Prepared windows build.");
    }

    [MenuItem("Deploy/Prepare Quest build")]
    private static void PrepareQuestBuild()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, MainGamePackageName);

        Debug.Log("Prepared Oculus Quest build.");
    }

    [MenuItem("Deploy/Prepare Quest build (DEMO)")]
    private static void PrepareQuestBuildDemo()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, DemoGamePackageName);

        Debug.Log("Prepared Oculus Quest DEMO build.");
    }

    [MenuItem("Deploy/Setup development settings")]
    private static void SetupDevelopmentSettings()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, MainGamePackageName);

        Debug.Log("Prepared development settings.");
    }

    [MenuItem("Deploy/Setup development settings (DEMO)")]
    private static void SetupDemoDevelopmentSettings()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, DemoGamePackageName);

        Debug.Log("Prepared development settings.");
    }
}
