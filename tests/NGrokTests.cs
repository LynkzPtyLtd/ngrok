using Xunit.Abstractions;

namespace Lynkz.NGrok.Tests;

public class NGrokTests : IClassFixture<TestNGrokResource>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly NGrokTestResource _ngrok;

    public NGrokTests(ITestOutputHelper testOutputHelper, TestNGrokResource ngrok)
    {
        _testOutputHelper = testOutputHelper;
        _ngrok = ngrok;
    }

    [Fact]
    public async Task Test()
    {
        var tunnel = await _ngrok.GetTunnel("Test");
        
        _testOutputHelper.WriteLine(tunnel?.PublicUrl);
        
        Assert.Equal("Test", tunnel?.Name);
        Assert.NotNull(tunnel?.PublicUrl);
    }
}