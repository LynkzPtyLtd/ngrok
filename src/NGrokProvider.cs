using System.Collections.Concurrent;

namespace Lynkz.NGrok;

public class NGrokProvider
{
    private static readonly ConcurrentDictionary<string, Lazy<NGrokServiceProvider>> ServiceProviders = new();

    public static NGrokServiceProvider Get(string configKey, Func<NGrokServiceProvider> setup)
    {
        var setupFactory = ServiceProviders.GetOrAdd(configKey,
            _ => new Lazy<NGrokServiceProvider>(setup));

        return setupFactory.Value;
    }
}