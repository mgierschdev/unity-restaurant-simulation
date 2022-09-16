using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Script used for the Jenkins Build
class BuildScript
{
    public static void PerformAndroidBuild()
    {
        string[] defaultScene = {
            "Assets/Scenes/World.unity",
            };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = defaultScene;
        buildPlayerOptions.locationPathName = ".";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    public static void PerformIOSBuild()
    {
        string[] defaultScene = {
            "Assets/Scenes/World.unity",
            };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = defaultScene;
        buildPlayerOptions.locationPathName = ".";
        buildPlayerOptions.target = BuildTarget.;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}