using System.Text.Json;
using System.Text.RegularExpressions;
using EToro.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EToro.Client;

public sealed partial class EToroClient : IEToroClient
{
    private static readonly JsonSerializerOptions JsonOptions = EToroJsonContext.Default.Options;
    private readonly HttpClient _httpClient;
    private readonly ErrorHandlingOptions _errorHandlingOptions;

    public EToroClient(HttpClient httpClient)
        : this(httpClient, Options.Create(new EToroOptions()))
    {
    }

    [ActivatorUtilitiesConstructor]
    public EToroClient(HttpClient httpClient, IOptions<EToroOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _errorHandlingOptions = options.Value.ErrorHandling ?? new ErrorHandlingOptions();
    }

    private static string ToEnvSegment(Models.Common.EToroEnvironment env) =>
        env == Models.Common.EToroEnvironment.Demo ? "demo" : "real";

    private async Task<T> GetFromJsonAsync<T>(
        string url,
        string emptyResponseMessage,
        CancellationToken ct)
    {
        using var response = await _httpClient.GetAsync(url, ct);
        await EnsureSuccessAsync(response, ct);

        var body = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidOperationException(emptyResponseMessage);

        return JsonSerializer.Deserialize<T>(body, JsonOptions)
               ?? throw new InvalidOperationException(emptyResponseMessage);
    }

    private async Task<T?> GetFromJsonOrNoContentAsync<T>(
        string url,
        string emptyResponseMessage,
        CancellationToken ct)
    {
        using var response = await _httpClient.GetAsync(url, ct);
        if ((int)response.StatusCode == 204)
            return default;

        await EnsureSuccessAsync(response, ct);

        var body = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidOperationException(emptyResponseMessage);

        return JsonSerializer.Deserialize<T>(body, JsonOptions)
               ?? throw new InvalidOperationException(emptyResponseMessage);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = !_errorHandlingOptions.IncludeResponseBody || response.Content is null
            ? null
            : FormatResponseBody(await response.Content.ReadAsStringAsync(ct), _errorHandlingOptions);

        throw new EToroApiException(
            response.StatusCode,
            GetSanitizedEndpoint(response),
            body,
            GetRequestId(response),
            GetRetryAfter(response));
    }

    private static string GetSanitizedEndpoint(HttpResponseMessage response)
    {
        var requestUri = response.RequestMessage?.RequestUri;
        if (requestUri is null)
            return "<unknown>";

        return requestUri.IsAbsoluteUri ? requestUri.PathAndQuery : requestUri.ToString();
    }

    private static string? GetRequestId(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("x-request-id", out var requestIds))
            return requestIds.FirstOrDefault();

        if (response.Headers.TryGetValues("request-id", out requestIds))
            return requestIds.FirstOrDefault();

        if (response.RequestMessage?.Headers.TryGetValues("x-request-id", out requestIds) == true)
            return requestIds.FirstOrDefault();

        return null;
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        var retryAfter = response.Headers.RetryAfter;

        if (retryAfter?.Delta is { } delta)
            return delta;

        if (retryAfter?.Date is { } date)
        {
            var wait = date - DateTimeOffset.UtcNow;
            return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
        }

        return null;
    }

    private static string FormatResponseBody(string body, ErrorHandlingOptions options)
    {
        var formatted = body.Trim();

        if (options.RedactResponseBody)
        {
            formatted = JsonSecretRegex().Replace(formatted, "${prefix}\"[REDACTED]\"");
            formatted = TextSecretRegex().Replace(formatted, "${prefix}[REDACTED]");
        }

        return options.MaxResponseBodyLength is { } maxLength && formatted.Length > maxLength
            ? formatted[..maxLength]
            : formatted;
    }

    [GeneratedRegex(
        "(?<prefix>[\"']?(?:apiKey|userKey|token|access_token|refresh_token|authorization|secret|password|x-api-key|x-user-key)[\"']?\\s*:\\s*)[\"'](?:\\\\.|[^\"'\\\\])*[\"']",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex JsonSecretRegex();

    [GeneratedRegex(
        "(?<prefix>\\b(?:apiKey|userKey|token|access_token|refresh_token|authorization|secret|password|x-api-key|x-user-key)\\b\\s*[=:]\\s*)[^\\s,;}&\\]]+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex TextSecretRegex();
}
