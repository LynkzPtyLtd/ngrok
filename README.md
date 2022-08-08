# Lynkz.NGrok
## Overview
NGrok xUnit module to support webhook or inbound web request integration testing

## Usage

### 1. Create an NGrok resource
```
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
```

### 2. Add the NGrok resource to your Test classes
```
public class NGrokTests : IClassFixture<TestNGrokResource>
```

### 3. Access the NGrok resource via Constructor Injection
```
public NGrokTests(ITestOutputHelper testOutputHelper, TestNGrokResource ngrok) { ... }
```

### 4. Access the established NGrok tunnels from your tests
```
[Fact]
public async Task Test()
{
    var tunnel = await _ngrok.GetTunnel("Test");
    
    Assert.NotNull(tunnel?.PublicUrl);
}
```
