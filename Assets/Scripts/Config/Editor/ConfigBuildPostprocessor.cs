using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class ConfigBuildPostprocessor
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log($"ConfigBuildPostprocessor.OnPostprocessBuild called for target {target} at path {pathToBuiltProject}");

        string sourceDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Config"));
        string buildDirectory = Path.GetDirectoryName(pathToBuiltProject);
        string targetDirectory = Path.Combine(buildDirectory, "Config");

        if (!Directory.Exists(sourceDirectory))
        {
            Debug.LogWarning($"Config directory not found: {sourceDirectory}");
            return;
        }

        CopyDirectory(sourceDirectory, targetDirectory);

        Debug.Log($"Config copied to build: {targetDirectory}");
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);

        foreach (string file in Directory.GetFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);

        foreach (string directory in Directory.GetDirectories(source))
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
    }
}