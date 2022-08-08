using System.Text.Json.Serialization;

namespace Lynkz.NGrok.Models;

public abstract class NGrokBaseMetrics
{
    public int Count { get; set; }
    public double Rate1 { get; set; }
    public double Rate5 { get; set; }
    public double Rate15 { get; set; }

    [JsonPropertyName("p50")]
    public double Percentile50 { get; set; }

    [JsonPropertyName("p90")]
    public double Percentile90 { get; set; }

    [JsonPropertyName("p95")]
    public double Percentile95 { get; set; }

    [JsonPropertyName("p99")]
    public double Percentile99 { get; set; }
}