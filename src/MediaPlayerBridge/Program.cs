using MediaPlayerBridge;

if (args.Length == 0)
{
    ApplicationConfiguration.Initialize();
    Application.Run(new SetupForm());
    return;
}

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
        "MediaPlayerBridge\n\nDouble-click the EXE to open setup.",
        "MediaPlayerBridge");
    return;
}

using var api = new LocalApiServer();
using var bridge = new MediaBridgeService();
api.Start();
await bridge.StartAsync();

Application.Run();
