using MediaPlayerBridge;

if (args.Length > 0)
{
    var command = args[0].Trim().ToLowerInvariant();
    if (command == "--install-startup")
    {
        Startup.Install();
        return;
    }

    if (command == "--install-pixel")
    {
        RainmeterInstaller.InstallPixel();
        return;
    }

    if (command == "--remove-startup")
    {
        Startup.Remove();
        return;
    }

    if (command == "--uninstall")
    {
        Installation.Uninstall();
        return;
    }

    if (command is "toggle" or "next" or "previous" or "stop")
    {
        await MediaSessionCommands.ExecuteAsync(command);
        return;
    }

    if (command == "--help")
    {
        MessageBox.Show(
            "MediaPlayerBridge\n\n--install-pixel\n--install-startup\n--remove-startup\n--uninstall",
            "MediaPlayerBridge");
        return;
    }
}

using var api = new LocalApiServer();
using var bridge = new MediaBridgeService();
api.Start();
await bridge.StartAsync();

Application.Run();
