namespace MediaPlayerBridge;

internal static class Installation
{
    public static void Uninstall()
    {
        RainmeterInstaller.UninstallPixel();
        Startup.Remove();

        var dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MediaPlayerBridge");

        if (Directory.Exists(dataDirectory))
        {
            Directory.Delete(dataDirectory, true);
        }
    }
}
