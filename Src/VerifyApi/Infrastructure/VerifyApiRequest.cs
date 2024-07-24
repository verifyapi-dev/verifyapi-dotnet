using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace VerifyApi.Infrastructure;

public class VerifyApiRequest(IVerifyApiClient client, HttpMethod method, string path, object body)
{
    /// <summary>
    /// The HTTP method for the request (POST, GET, etc.)
    /// </summary>
    public HttpMethod Method { get; } = method;

    /// <summary>
    /// The full URL for the request, which also includes query string parameters
    /// </summary>
    public Uri Uri { get; } = BuildUri(client, path);

    /// <summary>
    /// The value of the <c>Authorization</c> header with the API key
    /// </summary>
    public AuthenticationHeaderValue AuthorizationHeader { get; } = BuildAuthorizationHeader(client);

    /// <summary>
    /// The body of the request
    /// </summary>
    /// <remarks>This getter creates a new instance every time it is called.</remarks>
    public HttpContent Content => BuildContent(Method, body);

    private static AuthenticationHeaderValue BuildAuthorizationHeader(IVerifyApiClient client)
    {
        var apiKey = client.ApiKey;

        if (apiKey == null)
            throw new VerifyApiException("No API key provided.");

        return new AuthenticationHeaderValue("Bearer", apiKey);
    }

    private static HttpContent BuildContent(HttpMethod method, object body)
    {
        if (method != HttpMethod.Post)
            return new StringContent("");

        return new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
    }

    private static Uri BuildUri(IVerifyApiClient client, string path)
    {
        var b = new StringBuilder();

        b.Append(client.ApiBaseUrl);
        b.Append(path);

        return new Uri(b.ToString());
    }
}