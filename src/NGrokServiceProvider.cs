using Microsoft.Extensions.DependencyInjection;

namespace Lynkz.NGrok;

public class NGrokServiceProvider
{
    public ServiceProvider ServiceProvider { get; init; } = default!;
    public NGrokProcess Process { get; init; } = default!;
}