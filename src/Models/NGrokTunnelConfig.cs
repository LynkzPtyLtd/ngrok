using System.Text.Json.Serialization;

namespace Lynkz.NGrok.Models;

public class NGrokTunnelConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "command_line";

    /// <summary>
    ///     Tunnel protocol name, one of 'http', 'tcp', 'tls'
    /// </summary>
    [JsonPropertyName("proto")]
    public string Protocol { get; set; } = default!;

    /// <summary>
    ///     Forward traffic to this local port number or network address
    /// </summary>
    [JsonPropertyName("addr")]
    public string Address { get; set; } = default!;

    /// <summary>
    ///     Enable http request inspection
    /// </summary>
    public bool Inspect { get; set; } = true;

    /// <summary>
    ///     HTTP basic authentication credentials to enforce on tunneled requests
    /// </summary>
    [JsonPropertyName("basic_auth")]
    public string HttpBasicAuthenticationCredentials { get; set; } = default!;

    /// <summary>
    ///     Rewrite the HTTP Host header to this value, or 'preserve' to leave it unchanged
    /// </summary>
    [JsonPropertyName("host_header")]
    public string HttpHostHeader { get; set; } = default!;
    
    /// <summary>
    ///     Subdomain name to request. If unspecified, uses the tunnel name
    /// </summary>
    [JsonPropertyName("subdomain")]
    public string HttpTlsSubdomain { get; set; } = default!;

    /// <summary>
    ///     Hostname to request (requires reserved name and DNS CNAME)
    /// </summary>
    [JsonPropertyName("hostname")]
    public string HttpTlsHostname { get; set; } = default!;
    
    /// <summary>
    ///     Bind the remote TCP port on the given address
    /// </summary>
    [JsonPropertyName("remote_addr")]
    public string TcpRemoteAddress { get; set; } = default!;
}