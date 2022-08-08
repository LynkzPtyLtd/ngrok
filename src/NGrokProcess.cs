using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Lynkz.NGrok;

public class NGrokProcess : IAsyncDisposable
{
    public static readonly NGrokProcess Instance = new();

    private Process? _process;
    private string? _configPath;
    private CancellationTokenSource? _processTokenSource;

    private NGrokProcess()
    {
        _processTokenSource = new CancellationTokenSource();
    }

    public async Task ConnectAsync(Uri apiUri, string path, CancellationToken cancellationToken = new())
    {
        var ngrokProcesses = Process.GetProcessesByName("ngrok");
        if (ngrokProcesses.Length > 0)
        {
            var canConnect = await TestConnection(apiUri, cancellationToken).ConfigureAwait(false);
            if (canConnect)
            {
                await WaitForReady(apiUri, cancellationToken).ConfigureAwait(false);
            }
        }

        var executablePath = new FileInfo(path);
        var workingDirectory = executablePath.Directory!;

        _ = Task.Run(async () =>
        {
            _configPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            var sb = new StringBuilder();
            sb.AppendLine("version: 2");
            sb.AppendLine($"web_addr: {apiUri.Host}:{apiUri.Port}");

            await using (var fileStream = new StreamWriter(File.Open(_configPath, FileMode.Create)))
            {
                await fileStream.WriteLineAsync(sb, cancellationToken).ConfigureAwait(false);
            }

            _process = new Process
            {
                StartInfo = new ProcessStartInfo(executablePath.FullName)
                {
                    WorkingDirectory = workingDirectory.FullName,
                    Arguments = $"start --none --config {_configPath}",
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                }
            };

            try
            {
                Console.WriteLine("Starting NGrok");
                _process.Start();

                await _process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"NGrok exited {_process.ExitCode}");
            }
            catch (TaskCanceledException)
            {
                // Do nothing
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _processTokenSource?.Cancel();
            }
        }, cancellationToken);

        await WaitForReady(apiUri, cancellationToken).ConfigureAwait(false);
    }

    private async Task WaitForReady(Uri apiUri, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
    }

    private static async Task<bool> TestConnection(Uri apiUri, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
        
        using var tcpClient = new TcpClient();
        while (!linkedToken.IsCancellationRequested)
        {
            try
            {
                await tcpClient.ConnectAsync(apiUri.Host, apiUri.Port, cts.Token).ConfigureAwait(false);
                return true;
            }
            catch (TaskCanceledException) when (!linkedToken.IsCancellationRequested)
            {
                // Do nothing
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused)
            {
                // Do nothing
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100), linkedToken.Token).ConfigureAwait(false);
        }

        return false;
    }

    public async ValueTask DisposeAsync()
    {
        await Close().ConfigureAwait(false);
        
        _processTokenSource?.Dispose();
        _processTokenSource = default!;
    }

    private Task Close()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                File.Delete(_configPath);
            }
        }
        catch (Exception)
        {
            // Do nothing
        }

        var ngrokProcesses = Process.GetProcessesByName("ngrok");
        for (var i = 0; i < ngrokProcesses.Length; i++)
        {
            try
            {
                ngrokProcesses[i].Refresh();
                ngrokProcesses[i].CloseMainWindow();
                ngrokProcesses[i].Kill();
            }
            catch (Exception)
            {
                // Do nothing
            }
        }

        return Task.CompletedTask;
    }
}