using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Script used for the Jenkins Build
public class BuildScript
{
    static void PerformBuild()
    {
        string[] defaultScene = {
            "Assets/Scenes/World.unity",
            };

        BuildPipeline.BuildPlayer(defaultScene, "TestBuild.apk",
            BuildTarget.Android, BuildOptions.None);
    }
    // static void PerformAndroidBuild()
    // {
    //     string[] defaultScene = {
    //         "Assets/Scenes/World.unity",
    //         };

    //     BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    //     buildPlayerOptions.scenes = defaultScene;
    //     buildPlayerOptions.locationPathName = "AndroidBuild" + System.DateTime.Now;
    //     buildPlayerOptions.target = BuildTarget.Android;
    //     buildPlayerOptions.options = BuildOptions.None;
    //     BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

    //     BuildSummary summary = report.summary;
    //     if (summary.result == BuildResult.Succeeded)
    //     {
    //         Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
    //     }

    //     if (summary.result == BuildResult.Failed)
    //     {
    //         Debug.Log("Build failed");
    //     }
    // }

    // statics void PerformIOSBuild()
    // {
    //     string[] defaultScene = {
    //         "Assets/Scenes/World.unity",
    //         };

    //     BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    //     buildPlayerOptions.scenes = defaultScene;
    //     buildPlayerOptions.locationPathName = "iOSBuild" + System.DateTime.Now;
    //     buildPlayerOptions.target = BuildTarget.iOS;
    //     buildPlayerOptions.options = BuildOptions.None;

    //     BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
    //     BuildSummary summary = report.summary;
    //     if (summary.result == BuildResult.Succeeded)
    //     {
    //         Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
    //     }

    //     if (summary.result == BuildResult.Failed)
    //     {
    //         Debug.Log("Build failed");
    //     }
    // }
}