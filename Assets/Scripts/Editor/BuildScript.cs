using System;
using UnityEditor;

public class BuildScript
{
    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildWindowsClient();
        BuildWindowsServer();
        BuildLinuxServer();
    }

    [MenuItem("Build/Build Server (Windows)")]
    public static void BuildWindowsServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Offline.unity","Assets/Scenes/TestingMultiplayer.unity" };
        buildPlayerOptions.locationPathName = "Builds/Windows/Server/Server.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;

        Console.WriteLine("Building Server (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Server (Windows).");
    }

    [MenuItem("Build/Build Server (Linux)")]
    public static void BuildLinuxServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = "Builds/Linux/Server/Server.x86_64",
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }


    [MenuItem("Build/Build Client (Windows)")]
    public static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Offline.unity", "Assets/Scenes/TestingMultiplayer.unity" },
            locationPathName = "Builds/Windows/Client/Client.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.CompressWithLz4HC
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}