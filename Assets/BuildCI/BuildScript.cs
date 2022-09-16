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
    static void PerformAndroidBuild()
    {
        string[] defaultScene = {
            "Assets/Scenes/World.unity",
            };

        BuildPipeline.BuildPlayer(defaultScene, "TestBuild.apk",
            BuildTarget.Android, BuildOptions.None);
    }

    static void PerformIOSBuild()
    {
        string[] defaultScene = {
            "Assets/Scenes/World.unity",
            };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
        buildPlayerOptions.locationPathName = "iOSBuild"+Time.deltaTime;
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