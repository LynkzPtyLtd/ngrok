using System.Net.Http.Json;
using Lynkz.NGrok.Models;

namespace Lynkz.NGrok;

public class DefaultNGrokHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var error = await response.Content.ReadFromJsonAsync<NGrokError>(cancellationToken: cancellationToken).ConfigureAwait(false);
        throw new NGrokException(response.StatusCode, error);
    }
}