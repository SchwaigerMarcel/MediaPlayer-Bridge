namespace MediaPlayerBridge;

internal static class Startup
{
    private static string CommandFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Startup),
        "MediaPlayerBridge.cmd");

    public static void Install()
    {
        var executable = Environment.ProcessPath
            ?? throw new InvalidOperationException("The executable path could not be determined.");

        var content = $"@echo off{Environment.NewLine}\"{executable}\" --background{Environment.NewLine}";
        File.WriteAllText(CommandFilePath, content);
    }

    public static void Remove()
    {
        if (File.Exists(CommandFilePath))
        {
            File.Delete(CommandFilePath);
        }
    }
}
