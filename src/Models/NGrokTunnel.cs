using System.Text.Json.Serialization;

namespace Lynkz.NGrok.Models;

public class NGrokTunnel
{
    public string Name { get; set; } = default!;

    public string Uri { get; set; } = default!;

    [JsonPropertyName("public_url")]
    public string PublicUrl { get; set; } = default!;

    public string Proto { get; set; } = default!;

    public NGrokTunnelConfig Config { get; set; } = default!;

    public NGrokMetrics Metrics { get; set; } = default!;
}