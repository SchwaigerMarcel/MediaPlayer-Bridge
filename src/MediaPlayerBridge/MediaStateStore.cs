namespace MediaPlayerBridge;

internal static class MediaStateStore
{
    private static MediaSnapshot _current = new(
        "", "", "", "", "", "Closed", "stop", "00:00", "00:00", 0, 0, "",
        false, false, false, false, DateTimeOffset.UtcNow);

    public static MediaSnapshot Current => Volatile.Read(ref _current);

    public static void Set(MediaSnapshot snapshot) => Volatile.Write(ref _current, snapshot);
}
