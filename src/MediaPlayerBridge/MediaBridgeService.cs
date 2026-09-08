using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media.Control;

namespace MediaPlayerBridge;

internal sealed class MediaBridgeService : IDisposable
{
    private readonly RainmeterWriter _writer = new();
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private GlobalSystemMediaTransportControlsSessionManager? _manager;
    private GlobalSystemMediaTransportControlsSession? _activeSession;
    private readonly CancellationTokenSource _stop = new();

    public async Task StartAsync()
    {
        _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        _manager.CurrentSessionChanged += (_, _) => _ = RefreshAsync();
        await RefreshAsync();
    }

    public void Stop() => _stop.Cancel();

    private async Task RefreshAsync()
    {
        if (_stop.IsCancellationRequested || _manager is null || !await _refreshLock.WaitAsync(0))
        {
            return;
        }

        try
        {
            var session = _manager.GetCurrentSession();
            if (session is null)
            {
                await PublishAsync(EmptySnapshot());
                return;
            }

            if (!ReferenceEquals(session, _activeSession))
            {
                if (_activeSession is not null)
                {
                    _activeSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                    _activeSession.PlaybackInfoChanged -= OnPlaybackInfoChanged;
                    _activeSession.TimelinePropertiesChanged -= OnTimelinePropertiesChanged;
                }

                _activeSession = session;
                session.MediaPropertiesChanged += OnMediaPropertiesChanged;
                session.PlaybackInfoChanged += OnPlaybackInfoChanged;
                session.TimelinePropertiesChanged += OnTimelinePropertiesChanged;
            }

            var properties = await session.TryGetMediaPropertiesAsync();
            var playback = session.GetPlaybackInfo();
            var timeline = session.GetTimelineProperties();
            var controls = playback.Controls;
            var status = playback.PlaybackStatus.ToString();

            var coverPath = await WriteCoverAsync(properties.Thumbnail)
                ?? MediaStateStore.Current.CoverPath;

            var snapshot = new MediaSnapshot(
                session.SourceAppUserModelId ?? "",
                properties.Title ?? "",
                properties.Artist ?? "",
                properties.AlbumTitle ?? "",
                properties.Genres.FirstOrDefault() ?? "",
                status,
                ToStateIcon(status),
                FormatTime(timeline.Position),
                FormatTime(timeline.EndTime),
                Math.Max(timeline.Position.TotalSeconds, 0),
                Math.Max(timeline.EndTime.TotalSeconds, 0),
                coverPath,
                controls.IsPlayEnabled,
                controls.IsPauseEnabled,
                controls.IsNextEnabled,
                controls.IsPreviousEnabled,
                DateTimeOffset.UtcNow);

            await PublishAsync(snapshot);
        }
        catch (Exception)
        {
            // A session can disappear while Windows is switching apps. Its next event retries.
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task PublishAsync(MediaSnapshot snapshot)
    {
        MediaStateStore.Set(snapshot);
        await _writer.WriteAsync(snapshot);
    }

    private static MediaSnapshot EmptySnapshot() => new(
        "", "", "", "", "", "Closed", "stop", "00:00", "00:00", 0, 0, "",
        false, false, false, false, DateTimeOffset.UtcNow);

    private static string ToStateIcon(string status) => status switch
    {
        "Playing" => "pause",
        "Paused" => "play",
        _ => "stop"
    };

    private static string FormatTime(TimeSpan value) =>
        value.TotalHours >= 1 ? value.ToString(@"h\:mm\:ss") : value.ToString(@"mm\:ss");

    private async void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args) =>
        await RefreshAsync();

    private async void OnPlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args) =>
        await RefreshAsync();

    private async void OnTimelinePropertiesChanged(GlobalSystemMediaTransportControlsSession sender, TimelinePropertiesChangedEventArgs args) =>
        await RefreshAsync();

    private async Task<string?> WriteCoverAsync(Windows.Storage.Streams.IRandomAccessStreamReference? thumbnail)
    {
        if (thumbnail is null)
        {
            return null;
        }

        var coverPath = _writer.CreateCoverPath();
        await using var input = (await thumbnail.OpenReadAsync()).AsStreamForRead();
        await using var output = new FileStream(coverPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        await input.CopyToAsync(output, _stop.Token);
        _writer.RemoveOldCovers();
        return coverPath;
    }

    public void Dispose()
    {
        _stop.Cancel();
        _stop.Dispose();
        _refreshLock.Dispose();
    }
}
