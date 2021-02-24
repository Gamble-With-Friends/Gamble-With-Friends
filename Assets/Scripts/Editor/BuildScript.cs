using UnityEditor;

public class BuildScript
{

    static readonly string[] SCENES = new string[] { "Assets/Scenes/Offline.unity", "Assets/Scenes/Casino.unity" };

    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildWindowsClient();
        BuildWindowsServer();
    }

    [MenuItem("Build/Build Server (Windows)")]
    public static void BuildWindowsServer()
    {
        BuildPlayerOptions buildPlayerOptions = getWindowsBuild("Builds/Windows/Server/Server.exe");
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Build/Build Client (Windows)")]
    public static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = getWindowsBuild("Builds/Windows/Client/Client.exe");
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static BuildPlayerOptions getWindowsBuild(string path)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = SCENES,
            locationPathName = path,
            target = BuildTarget.StandaloneWindows64,
        };

        return buildPlayerOptions;
    }
}