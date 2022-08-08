using System.Text.Json.Serialization;

namespace Lynkz.NGrok.Models;

public class NGrokErrorDetails
{
    [JsonPropertyName("err")]
    public string Error { get; set; } = default!;
}