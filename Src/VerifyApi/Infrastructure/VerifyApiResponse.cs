using System.Net;
using System.Net.Http.Headers;

namespace VerifyApi.Infrastructure;

public class VerifyApiResponse(HttpStatusCode statusCode, HttpResponseHeaders headers, string content)
{
    internal int NumRetries { get; set; }

    /// <summary>
    /// Gets the HTTP status code of the response
    /// </summary>
    /// <value>The HTTP status code of the response</value>
    public HttpStatusCode StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the HTTP headers of the response
    /// </summary>
    /// <value>The HTTP headers of the response</value>
    public HttpResponseHeaders Headers { get; } = headers;

    /// <summary>
    /// Gets the body of the response
    /// </summary>
    /// <value>The body of the response</value>
    public string Content { get; } = content;

    /// <summary>
    /// Returns a string that represents the <see cref="VerifyApiResponse"/> object
    /// </summary>
    /// <returns>A string that represents the <see cref="VerifyApiResponse"/> object</returns>
    public override string ToString()
    {
        return $"{GetType().FullName} status={StatusCode}";
    }
}