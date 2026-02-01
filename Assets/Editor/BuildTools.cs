#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Code.Editor
{
    public static class BuildTools
    {
        private const string WindowsFolder = "Builds/Windows";
        private const string WindowsExeName = "PharmaClicker.exe";
        private const string LinuxFolder = "Builds/Linux";
        private const string LinuxBinaryName = "PharmaClicker.x86_64";

        [MenuItem("Tools/Build/Windows64")]
        public static void BuildWindows64()
        {
            var buildPath = Path.Combine(WindowsFolder, WindowsExeName);
            Directory.CreateDirectory(WindowsFolder);

            var options = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = buildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"Build failed: {report.summary.result}");
            }
        }

        [MenuItem("Tools/Build/Linux64")]
        public static void BuildLinux64()
        {
            var buildPath = Path.Combine(LinuxFolder, LinuxBinaryName);
            Directory.CreateDirectory(LinuxFolder);

            var options = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = buildPath,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"Build failed: {report.summary.result}");
            }
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            var enabledCount = 0;
            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    enabledCount++;
                }
            }

            var enabledScenes = new string[enabledCount];
            var index = 0;
            foreach (var scene in scenes)
            {
                if (!scene.enabled)
                {
                    continue;
                }

                enabledScenes[index++] = scene.path;
            }

            return enabledScenes;
        }
    }
}
#endif
