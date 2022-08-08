using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lynkz.NGrok.Contracts;
using Lynkz.NGrok.Models;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Xunit;

namespace Lynkz.NGrok;

public abstract class NGrokTestResource : IAsyncLifetime
{
    private NGrokServiceProvider? _serviceProvider;
    private IServiceScope? _scope;
    private NGrokClientApi? _api;

    private readonly ConcurrentStack<NGrokTunnel> _tunnels = new();

    public async Task<NGrokTunnel?> CreateTunnel(NGrokTunnelConfig config, CancellationToken cancellationToken = new())
    {
        if (_api is null)
        {
            throw new InvalidOperationException("NGrok Api not connected");
        }
        
        var tunnel = await _api.CreateTunnel(config, cancellationToken).ConfigureAwait(false);
        if (tunnel is not null)
        {
            _tunnels.Push(tunnel);
        }

        return tunnel;
    }

    public async Task<NGrokTunnel?> GetTunnel(string name, CancellationToken cancellationToken = new())
    {
        if (_api is null)
        {
            throw new InvalidOperationException("NGrok Api not connected");
        }

        return await _api.GetTunnel(name, cancellationToken).ConfigureAwait(false);
    }

    protected virtual Task BeforeTest() => Task.CompletedTask;

    protected virtual Task AfterTest() => Task.CompletedTask;

    protected virtual string ExecutablePath => GetExecutablePath();

    private string GetExecutablePath()
    {
        if (Environment.Is64BitProcess && OperatingSystem.IsWindows())
        {
            return "binaries/win-x64/ngrok.exe";
        }

        if (Environment.Is64BitProcess && OperatingSystem.IsLinux())
        {
            return "binaries/linux-x64/ngrok";
        }

        throw new NotSupportedException();
    }

    protected virtual Uri ApiEndPoint => new("http://127.0.0.1:4040");

    protected virtual string ConfigKey => "Default";

    protected virtual void BeforeServicesSetup(IServiceCollection services)
    {
    }

    protected virtual void AfterServicesSetup(IServiceCollection services)
    {
    }

    protected virtual RefitSettings ConfigureRefitSettings(IServiceProvider serviceProvider)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        return new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(jsonSerializerOptions)
        };
    }

    protected virtual void ConfigureHttpClient(IServiceProvider serviceProvider, HttpClient client)
    {
        client.BaseAddress = ApiEndPoint;
    }

    public async Task InitializeAsync()
    {
        var ngrokInstance = NGrokProvider.Get(ConfigKey,
            () =>
            {
                var services = new ServiceCollection();

                BeforeServicesSetup(services);

                services
                    .AddRefitClient<NGrokClientApi>(ConfigureRefitSettings)
                    .ConfigurePrimaryHttpMessageHandler(ConfigureHttpMessageHandler)
                    .ConfigureHttpClient(ConfigureHttpClient);

                AfterServicesSetup(services);

                var serviceProvider = services.BuildServiceProvider();

                return new NGrokServiceProvider
                {
                    Process = NGrokProcess.Instance,
                    ServiceProvider = serviceProvider
                };
            });

        _serviceProvider = ngrokInstance;

        await _serviceProvider.Process.ConnectAsync(ApiEndPoint, ExecutablePath).ConfigureAwait(false);

        _scope = ngrokInstance.ServiceProvider.CreateScope();
        _api = _scope.ServiceProvider.GetRequiredService<NGrokClientApi>();

        await BeforeTest().ConfigureAwait(false);
    }

    protected virtual HttpMessageHandler ConfigureHttpMessageHandler(IServiceProvider serviceProvider)
    {
        return new DefaultNGrokHttpClientHandler();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        var exceptions = new List<Exception>();
        try
        {
            await AfterTest().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            exceptions.Add(e);
        }

        try
        {
            await CleanupTunnels(exceptions).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            exceptions.Add(e);
        }
        try
        {
            if (_scope is not null)
            {
                await new AsyncServiceScope(_scope).DisposeAsync().ConfigureAwait(false);
            }

            if (_serviceProvider is not null)
            {
                await _serviceProvider.Process.DisposeAsync().ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            exceptions.Add(e);
        }

        if (exceptions.Count == 1)
        {
            throw exceptions[0];
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }

    private async Task CleanupTunnels(List<Exception> exceptions)
    {
        if (_api is null)
        {
            return;
        }

        while (!_tunnels.IsEmpty)
        {
            try
            {
                if (!_tunnels.TryPop(out var tunnel))
                {
                    continue;
                }
                
                await _api.DeleteTunnel(tunnel.Name).ConfigureAwait(false);
            }
            catch (NGrokException e) when (e.Error?.StatusCode == HttpStatusCode.NotFound)
            {
                // Do nothing
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }
    }
}