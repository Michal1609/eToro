using EToro.Configuration;
using Microsoft.Extensions.Options;

namespace EToro.Client;

public sealed class EToroAuthHandler(IOptions<EToroOptions> options) : DelegatingHandler
{
    private readonly EToroOptions _options = options.Value;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        SetHeader(request, "x-api-key", _options.ApiKey);
        SetHeader(request, "x-user-key", _options.UserKey);
        SetHeader(request, "x-request-id", Guid.NewGuid().ToString("N"));
        return base.SendAsync(request, cancellationToken);
    }

    private static void SetHeader(HttpRequestMessage request, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.AsSpan().ContainsAny('\r', '\n'))
            throw new InvalidOperationException($"Header '{name}' value is invalid.");

        request.Headers.Remove(name);
        request.Headers.TryAddWithoutValidation(name, value);
    }
}
