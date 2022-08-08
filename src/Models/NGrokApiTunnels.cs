namespace Lynkz.NGrok.Models;

public class NGrokApiTunnels
{
    public NGrokTunnel[] Tunnels { get; set; } = default!;
    public string Uri { get; set; } = default!;
}