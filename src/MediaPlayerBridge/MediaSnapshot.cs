namespace MediaPlayerBridge;

internal sealed record MediaSnapshot(
    string Source,
    string Title,
    string Artist,
    string Album,
    string Genre,
    string PlaybackStatus,
    string StateIcon,
    string PositionText,
    string DurationText,
    double PositionSeconds,
    double DurationSeconds,
    string CoverPath,
    bool CanPlay,
    bool CanPause,
    bool CanNext,
    bool CanPrevious,
    DateTimeOffset UpdatedAt);
