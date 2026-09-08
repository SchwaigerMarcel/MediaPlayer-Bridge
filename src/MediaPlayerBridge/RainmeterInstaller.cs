using System.Text;
using System.Text.RegularExpressions;

namespace MediaPlayerBridge;

internal static class RainmeterInstaller
{
    private const string AdapterMarker = "MediaPlayerBridge adapter";
    private static readonly string[] SkinFileNames =
    [
        "piXel Images.ini",
        "piXel VuMeter.ini",
        "piXel VuMeter RGB.ini",
        "piXel VuMeter Custom.ini"
    ];

    public static void InstallPixel()
    {
        var skinRoot = GetSkinRoot();
        if (!Directory.Exists(skinRoot))
        {
            throw new DirectoryNotFoundException(
                "piXel visualizer was not found under Documents\\Rainmeter\\Skins. Install the RMSKIN first.");
        }

        var measuresDirectory = Path.Combine(skinRoot, "@Resources", "Measures");
        var playerMeasures = Path.Combine(measuresDirectory, "mPlayer.inc");
        var backup = Path.Combine(measuresDirectory, "mPlayer.NowPlaying.backup.inc");
        Directory.CreateDirectory(measuresDirectory);

        if (File.Exists(playerMeasures) && !File.ReadAllText(playerMeasures).Contains(AdapterMarker, StringComparison.Ordinal))
        {
            File.Copy(playerMeasures, backup, true);
        }

        WriteEmbeddedAdapter(playerMeasures);

        var executable = Environment.ProcessPath
            ?? throw new InvalidOperationException("The executable path could not be determined.");

        foreach (var fileName in SkinFileNames)
        {
            PatchControls(Path.Combine(skinRoot, fileName), executable);
        }
    }

    public static void UninstallPixel()
    {
        var skinRoot = GetSkinRoot();
        if (!Directory.Exists(skinRoot))
        {
            return;
        }

        var measuresDirectory = Path.Combine(skinRoot, "@Resources", "Measures");
        var playerMeasures = Path.Combine(measuresDirectory, "mPlayer.inc");
        var backup = Path.Combine(measuresDirectory, "mPlayer.NowPlaying.backup.inc");

        if (File.Exists(backup))
        {
            File.Copy(backup, playerMeasures, true);
            File.Delete(backup);
        }

        foreach (var fileName in SkinFileNames)
        {
            RestoreControls(Path.Combine(skinRoot, fileName));
        }
    }

    private static void WriteEmbeddedAdapter(string targetPath)
    {
        const string resourceName = "MediaPlayerBridge.Rainmeter.mPlayer.MediaPlayerBridge.inc";
        using var stream = typeof(RainmeterInstaller).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The embedded Rainmeter adapter is missing.");
        using var reader = new StreamReader(stream, Encoding.UTF8);
        File.WriteAllText(targetPath, reader.ReadToEnd(), Encoding.UTF8);
    }

    private static string GetSkinRoot() => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Rainmeter",
        "Skins",
        "piXel visualizer");

    private static void PatchControls(string skinPath, string executable)
    {
        if (!File.Exists(skinPath))
        {
            return;
        }

        var commandPrefix = "[\"" + executable + "\" ";
        var content = File.ReadAllText(skinPath)
            .Replace("[!CommandMeasure mTitulo \"PlayPause\"]", commandPrefix + "toggle]", StringComparison.Ordinal)
            .Replace("!CommandMeasure \"mTitulo\" \"Previous\"", commandPrefix + "previous]", StringComparison.Ordinal)
            .Replace("!CommandMeasure \"mTitulo\" \"Next\"", commandPrefix + "next]", StringComparison.Ordinal)
            .Replace("!CommandMeasure \"mTitulo\" \"Stop\"", commandPrefix + "stop]", StringComparison.Ordinal)
            .Replace(
                "H=128\r\nMouseOverAction=!ShowMeterGroup Info",
                "H=128\r\nDynamicVariables=1\r\nMouseOverAction=!ShowMeterGroup Info",
                StringComparison.Ordinal)
            .Replace(
                "H=128\nMouseOverAction=!ShowMeterGroup Info",
                "H=128\nDynamicVariables=1\nMouseOverAction=!ShowMeterGroup Info",
                StringComparison.Ordinal);

        File.WriteAllText(skinPath, content);
    }

    private static void RestoreControls(string skinPath)
    {
        if (!File.Exists(skinPath))
        {
            return;
        }

        var content = File.ReadAllText(skinPath);
        content = Regex.Replace(
            content,
            "\\[\\\"[^\\\"]*MediaPlayerBridge\\.exe\\\" toggle\\]",
            "[!CommandMeasure mTitulo \"PlayPause\"]",
            RegexOptions.IgnoreCase);
        content = Regex.Replace(
            content,
            "\\[\\\"[^\\\"]*MediaPlayerBridge\\.exe\\\" previous\\]",
            "!CommandMeasure \"mTitulo\" \"Previous\"",
            RegexOptions.IgnoreCase);
        content = Regex.Replace(
            content,
            "\\[\\\"[^\\\"]*MediaPlayerBridge\\.exe\\\" next\\]",
            "!CommandMeasure \"mTitulo\" \"Next\"",
            RegexOptions.IgnoreCase);
        content = Regex.Replace(
            content,
            "\\[\\\"[^\\\"]*MediaPlayerBridge\\.exe\\\" stop\\]",
            "!CommandMeasure \"mTitulo\" \"Stop\"",
            RegexOptions.IgnoreCase);

        File.WriteAllText(skinPath, content);
    }
}
