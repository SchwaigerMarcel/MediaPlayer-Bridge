using System.Net;
using System.Text.Json;

namespace MediaPlayerBridge;

internal sealed class LocalApiServer : IDisposable
{
    private readonly HttpListener _listener = new();
    private readonly CancellationTokenSource _stop = new();

    public LocalApiServer()
    {
        _listener.Prefixes.Add("http://127.0.0.1:8974/");
    }

    public void Start() => _ = ListenAsync();

    private async Task ListenAsync()
    {
        _listener.Start();

        try
        {
            while (!_stop.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();
                _ = RespondAsync(context);
            }
        }
        catch (HttpListenerException) when (_stop.IsCancellationRequested)
        {
        }
        catch (ObjectDisposedException) when (_stop.IsCancellationRequested)
        {
        }
    }

    private static async Task RespondAsync(HttpListenerContext context)
    {
        var path = context.Request.Url?.AbsolutePath ?? "/";
        if (path.Equals("/now-playing", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(MediaStateStore.Current);
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.ContentLength64 = bytes.Length;
            await context.Response.OutputStream.WriteAsync(bytes);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        context.Response.Close();
    }

    public void Dispose()
    {
        _stop.Cancel();
        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener.Close();
        _stop.Dispose();
    }
}
