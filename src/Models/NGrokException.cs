using System.Net;

namespace Lynkz.NGrok.Models;

public sealed class NGrokException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public NGrokError? Error { get; }
    
    public NGrokException()
        : this(default, default)
    {
    }

    public NGrokException(HttpStatusCode? statusCode, NGrokError? error)
        : base(CreateMessage(statusCode, error))
    {
        StatusCode = statusCode;
        Error = error;
        Data.Add("Error", error);
    }

    private static string CreateMessage(HttpStatusCode? statusCode, NGrokError? error)
    {
        if (error is null)
        {
            return nameof(NGrokException);
        }

        return "Status code: " + statusCode + "\n" + error.Message + "\n" + error.Details.Error;
    }
}