using System.Collections.Concurrent;
using EToro.Configuration;
using Microsoft.Extensions.Options;

namespace EToro.Client;

/// <summary>
/// Per-user-key sliding-window rate limiter for multi-user scenarios.
/// </summary>
/// <remarks>
/// The eToro API enforces rate limits per user key (60 RPM for standard read operations,
/// 20 RPM for write/transactional operations). When a single application instance serves
/// multiple user keys, each key must be throttled independently.
/// <para>
/// This implementation reads the <c>x-user-key</c> header set by <see cref="EToroAuthHandler"/>
/// and maintains a separate sliding window per user key. Keyed windows are created on first use
/// and held for the lifetime of this singleton instance.
/// </para>
/// <para>
/// Register via <c>AddEToro(..., useKeyedRateLimiter: true)</c>.
/// </para>
/// </remarks>
public sealed class EToroKeyedRateLimiter : IEToroRateLimiter
{
    private readonly ConcurrentDictionary<string, EToroSlidingWindow> _windows = new();
    private readonly RateLimitOptions _options;
    private readonly TimeProvider _timeProvider;

    public EToroKeyedRateLimiter(IOptions<EToroOptions> options)
        : this(options, TimeProvider.System)
    {
    }

    internal EToroKeyedRateLimiter(IOptions<EToroOptions> options, TimeProvider timeProvider)
    {
        _options = options.Value.RateLimit;
        _timeProvider = timeProvider;
    }

    public async Task WaitAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (!_options.Enabled || request.Method != HttpMethod.Get)
            return;

        var userKey = GetUserKey(request);
        var window = _windows.GetOrAdd(
            userKey,
            _ => new EToroSlidingWindow(_options.PermitLimit, _options.Window, _timeProvider));

        await window.WaitAsync(ct);
    }

    private static string GetUserKey(HttpRequestMessage request)
    {
        if (request.Headers.TryGetValues("x-user-key", out var values))
            return values.FirstOrDefault() ?? string.Empty;

        return string.Empty;
    }
}
