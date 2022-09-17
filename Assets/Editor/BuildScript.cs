using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;


// Script used for the Jenkins Build
class BuildScript
{
    public static void PerformAndroidBuild()
    {

        string[] defaultScene = {
            "Assets/World.unity",
            };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = defaultScene;
        buildPlayerOptions.locationPathName = "~/Unity/Android/android" + Util.GetUnixTimeNow() + ".apk";
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
            "Assets/World.unity",
            };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = defaultScene;
        buildPlayerOptions.locationPathName = "~/Unity/IOS/" + Util.GetUnixTimeNow() + "/";
        buildPlayerOptions.target = BuildTarget.iOS;
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