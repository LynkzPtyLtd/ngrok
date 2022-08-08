using System.Net;
using System.Text.Json.Serialization;

namespace Lynkz.NGrok.Models;

public class NGrokError
{
    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }
    
    [JsonPropertyName("status_code")]
    public HttpStatusCode StatusCode { get; set; } = default!;
    
    [JsonPropertyName("msg")]
    public string Message { get; set; } = default!;
    
    [JsonPropertyName("details")]
    public NGrokErrorDetails Details { get; set; } = default!;
}