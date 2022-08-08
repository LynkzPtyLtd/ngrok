using Lynkz.NGrok.Models;

namespace Lynkz.NGrok.Tests;

public class TestNGrokResource : NGrokTestResource
{
    protected override async Task BeforeTest()
    {
        await CreateTunnel(new NGrokTunnelConfig
        {
            Name = "Test",
            Address = "80",
            Protocol = "http"
        });
    }
}