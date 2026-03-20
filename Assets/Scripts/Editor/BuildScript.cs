#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void BuildWebGL()
    {
        var options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Game.unity" },
            locationPathName = "Builds/WebGL",
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"WebGL build succeeded: {report.summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"WebGL build failed: {report.summary.result}");
            EditorApplication.Exit(1);
        }
    }
}
#endif
