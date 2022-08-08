using System.Text.Json.Serialization;

namespace Lynkz.NGrok.Models;

public class NGrokMetrics
{
    public NGrokHttpMetrics Http { get; set; } = default!;

    [JsonPropertyName("conns")]
    public NGrokConnectionMetrics Connection { get; set; } = default!;
}