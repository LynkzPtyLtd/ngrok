using Lynkz.NGrok.Models;
using Refit;

namespace Lynkz.NGrok.Contracts;

public interface NGrokClientApi
{
    [Get("/api/tunnels/{name}")]
    Task<NGrokTunnel?> GetTunnel(string name, CancellationToken cancellationToken = new());
    
    [Post("/api/tunnels")]
    Task<NGrokTunnel?> CreateTunnel([Body(BodySerializationMethod.Serialized)] NGrokTunnelConfig config, CancellationToken cancellationToken = new());

    [Delete("/api/tunnels/{name}")]
    Task DeleteTunnel(string name, CancellationToken cancellationToken = new());
}