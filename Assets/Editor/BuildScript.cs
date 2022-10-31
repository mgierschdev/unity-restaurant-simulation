using UnityEditor;
using UnityEditor.Build.Reporting;

// Script used for the Jenkins Build
class BuildScript
{
    public static void PerformAndroidBuild()
    {

        string[] defaultScene = {
            "Assets/LoadScene.unity",
            "Assets/GameScene.unity"
            };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = defaultScene,
            locationPathName = "~/Unity/Android/android" + Util.GetUnixTimeNow() + ".apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            GameLog.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            GameLog.Log("Build failed");
        }
    }

    public static void PerformIOSBuild()
    {
        string[] defaultScene = {
            "Assets/LoadScene.unity",
            "Assets/GameScene.unity"
            };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = defaultScene,
            locationPathName = "~/Unity/IOS/" + Util.GetUnixTimeNow() + "/",
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            GameLog.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            GameLog.Log("Build failed");
        }
    }
}