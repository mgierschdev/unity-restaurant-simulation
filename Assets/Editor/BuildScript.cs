#if (UNITY_EDITOR)

using UnityEditor;
using UnityEditor.Build.Reporting;
using Util;

// Script used for the Jenkins/local Build
namespace Editor
{
    /**
     * Problem: Provide editor build automation for CI and local builds.
     * Goal: Build Android and iOS players from editor scripts.
     * Approach: Use Unity BuildPipeline with configured scene lists.
     * Time: O(n) with build complexity (n = assets/scripts).
     * Space: O(n) for build artifacts.
     */
    internal class BuildScript
    {
        public static void PerformAndroidBuild()
        {

            string[] defaultScene = {
                "Assets/Scenes/LoadScene.unity",
                "Assets/Scenes/GameScene.unity"
            };

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = defaultScene,
                locationPathName = "~/Unity/Android/android" + Util.Util.GetUnixTimeNow() + ".apk",
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
                "Assets/Scenes/LoadScene.unity",
                "Assets/Scenes/GameScene.unity"
            };

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = defaultScene,
                locationPathName = "~/Unity/IOS/" + Util.Util.GetUnixTimeNow() + "/",
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
}

#endif
