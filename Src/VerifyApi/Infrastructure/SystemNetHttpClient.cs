using System.Net;
using System.Net.Http;

namespace VerifyApi.Infrastructure;

internal class SystemNetHttpClient
{
    private static readonly Lazy<HttpClient> LazyDefaultHttpClient = new(BuildDefaultSystemNetHttpClient);

    private const string NetTargetFramework =
#if NET6_0
            "net6.0"
#elif NET8_0
             "net8.0"
#elif NETCOREAPP3_1
             "netcoreapp3.1"
#elif NETSTANDARD2_0
            "netstandard2.0"
#elif NET461
            "net461"
#else
#error "Unknown target framework"
#endif
        ;

    private readonly HttpClient _httpClient;

    private readonly object _randLock = new object();

    private readonly Random _rand = new Random();

    private readonly string _userAgentString;

    /// <summary>
    /// Maximum number of retries made by the client.
    /// </summary>
    public const int MaxNetworkNumberRetries = 2;

    /// <summary>
    /// Default timespan before the request times out.
    /// </summary>
    public static TimeSpan DefaultHttpTimeout => TimeSpan.FromSeconds(60);

    /// <summary>
    /// Maximum sleep time between tries to send HTTP requests after network failure.
    /// </summary>
    public static TimeSpan MaxNetworkRetriesDelay => TimeSpan.FromSeconds(5);

    /// <summary>
    /// Minimum sleep time between tries to send HTTP requests after network failure.
    /// </summary>
    public static TimeSpan MinNetworkRetriesDelay => TimeSpan.FromMilliseconds(500);

    static SystemNetHttpClient()
    {
        // Enable support for TLS 1.2, as VerifyAPI API requires it. This should only be
        // necessary for .NET Framework 4.5 as more recent runtimes should have TLS 1.2 enabled
        // by default, but it can be disabled in some environments.
        ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemNetHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> client to use. If <c>null</c>, an HTTP client will be created with default parameters.</param>
    public SystemNetHttpClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? LazyDefaultHttpClient.Value;
        _userAgentString = BuildUserAgentString();
    }

    /// <summary>
    /// Sends a request to VerifyAPI as an asynchronous operation.
    /// </summary>
    /// <param name="request">The parameters of the request to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task<VerifyApiResponse> MakeRequestAsync(VerifyApiRequest request, CancellationToken cancellationToken = default)
    {
        var (response, retries) = await SendHttpRequest(request, cancellationToken).ConfigureAwait(false);

        var reader = new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

        return new VerifyApiResponse(response.StatusCode, response.Headers, await reader.ReadToEndAsync().ConfigureAwait(false))
        {
            NumRetries = retries,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClient"/> class
    /// with default parameters.
    /// </summary>
    /// <returns>The new instance of the <see cref="HttpClient"/> class.</returns>
    public static HttpClient BuildDefaultSystemNetHttpClient()
    {
        return new HttpClient
        {
            Timeout = DefaultHttpTimeout,
        };
    }

    private async Task<(HttpResponseMessage response, int retries)> SendHttpRequest(VerifyApiRequest request, CancellationToken cancellationToken)
    {
        Exception? requestException;
        HttpResponseMessage? response = null;
        var retry = 0;


        while (true)
        {
            requestException = null;

            var httpRequest = BuildRequestMessage(request);

            try
            {
                response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException exception)
            {
                requestException = exception;
            }
            catch (OperationCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                requestException = exception;
            }


            if (!ShouldRetry(retry, requestException != null, response?.StatusCode))
                break;

            retry += 1;
            await Task.Delay(SleepTime(retry), cancellationToken).ConfigureAwait(false);
        }

        if (requestException != null)
            throw requestException;

        return (response, retry)!;
    }

    private string BuildUserAgentString()
    {
        return $"VerifyAPI 1.0 dotnet ({NetTargetFramework})";
    }

    private bool ShouldRetry(
        int numRetries,
        bool error,
        HttpStatusCode? statusCode)
    {
        // Do not retry if we are out of retries.
        if (numRetries >= MaxNetworkNumberRetries)
        {
            return false;
        }

        // Retry on connection error.
        if (error)
        {
            return true;
        }

        // Retry on conflict errors.
        if (statusCode == HttpStatusCode.Conflict)
        {
            return true;
        }

        // Retry on 500, 503, and other internal errors.
        if (statusCode.HasValue && (int)statusCode.Value >= 500)
        {
            return true;
        }

        return false;
    }

    private HttpRequestMessage BuildRequestMessage(VerifyApiRequest request)
    {
        var requestMessage = new HttpRequestMessage(request.Method, request.Uri);

        // Standard headers
        requestMessage.Headers.TryAddWithoutValidation("User-Agent", _userAgentString);
        requestMessage.Headers.Authorization = request.AuthorizationHeader;

        // Request body
        requestMessage.Content = request.Content;

        return requestMessage;
    }

    private TimeSpan SleepTime(int numRetries)
    {
        // Apply exponential backoff with MinNetworkRetriesDelay on the number of numRetries
        // so far as inputs.
        var delay = TimeSpan.FromTicks((long)(MinNetworkRetriesDelay.Ticks
                                              * Math.Pow(2, numRetries - 1)));

        // Do not allow the number to exceed MaxNetworkRetriesDelay
        if (delay > MaxNetworkRetriesDelay)
        {
            delay = MaxNetworkRetriesDelay;
        }

        // Apply some jitter by randomizing the value in the range of 75%-100%.
        var jitter = 1.0;
        lock (_randLock)
        {
            jitter = (3.0 + _rand.NextDouble()) / 4.0;
        }

        delay = TimeSpan.FromTicks((long)(delay.Ticks * jitter));

        // But never sleep less than the base sleep seconds.
        if (delay < MinNetworkRetriesDelay)
        {
            delay = MinNetworkRetriesDelay;
        }

        return delay;
    }
}