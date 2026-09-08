using Windows.Media.Control;

namespace MediaPlayerBridge;

internal static class MediaSessionCommands
{
    public static async Task ExecuteAsync(string command)
    {
        var manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        var session = manager.GetCurrentSession();
        if (session is null)
        {
            return;
        }

        switch (command)
        {
            case "toggle":
                await session.TryTogglePlayPauseAsync();
                break;
            case "next":
                await session.TrySkipNextAsync();
                break;
            case "previous":
                await session.TrySkipPreviousAsync();
                break;
            case "stop":
                await session.TryStopAsync();
                break;
        }
    }
}
