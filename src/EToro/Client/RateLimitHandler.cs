using System.Net;
using EToro.Configuration;
using Microsoft.Extensions.Options;

namespace EToro.Client;

public sealed class RateLimitHandler : DelegatingHandler
{
    private readonly Func<TimeSpan, CancellationToken, Task> _delay;
    private readonly IEToroRateLimiter _rateLimiter;
    private readonly RateLimitOptions _options;

    public RateLimitHandler(IOptions<EToroOptions> options)
        : this(options, new EToroRateLimiter(options), DelayAsync)
    {
    }

    public RateLimitHandler(
        IOptions<EToroOptions> options,
        IEToroRateLimiter rateLimiter)
        : this(options, rateLimiter, DelayAsync)
    {
    }

    internal RateLimitHandler(
        IOptions<EToroOptions> options,
        IEToroRateLimiter rateLimiter,
        Func<TimeSpan, CancellationToken, Task>? delay = null)
    {
        _options = options.Value.RateLimit;
        _rateLimiter = rateLimiter;
        _delay = delay ?? DelayAsync;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (true)
        {
            await _rateLimiter.WaitAsync(request, cancellationToken);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.TooManyRequests)
                return response;

            if (!CanRetry(request) || attempt >= _options.MaxRetries)
                return response;

            var delay = GetRetryDelay(response, attempt);
            response.Dispose();
            await _delay(delay, cancellationToken);

            // Rebuild request for retry because HttpRequestMessage is single-use.
            request = await CloneRequestAsync(request, cancellationToken);
            attempt++;
        }
    }

    private bool CanRetry(HttpRequestMessage request)
    {
        return IsIdempotentMethod(request.Method) || _options.RetryNonIdempotentRequests;
    }

    private static bool IsIdempotentMethod(HttpMethod method)
    {
        return method == HttpMethod.Get || method == HttpMethod.Head;
    }

    private TimeSpan GetRetryDelay(HttpResponseMessage response, int attempt)
    {
        var retryAfter = response.Headers.RetryAfter;
        TimeSpan delay;

        if (retryAfter?.Delta is { } delta)
        {
            delay = delta;
        }
        else if (retryAfter?.Date is { } date)
        {
            var wait = date - DateTimeOffset.UtcNow;
            delay = wait > TimeSpan.Zero ? wait : _options.DefaultRetryDelay;
        }
        else
        {
            delay = GetBackoffDelay(attempt);
        }

        return CapDelay(delay);
    }

    private TimeSpan GetBackoffDelay(int attempt)
    {
        var multiplier = Math.Pow(2, attempt);
        var delay = TimeSpan.FromMilliseconds(_options.DefaultRetryDelay.TotalMilliseconds * multiplier);

        if (_options.RetryJitterRatio <= 0)
            return delay;

        var jitter = delay.TotalMilliseconds * _options.RetryJitterRatio * Random.Shared.NextDouble();
        return TimeSpan.FromMilliseconds(delay.TotalMilliseconds + jitter);
    }

    private TimeSpan CapDelay(TimeSpan delay)
    {
        if (delay < TimeSpan.Zero)
            return TimeSpan.Zero;

        return delay <= _options.MaxRetryDelay ? delay : _options.MaxRetryDelay;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(
        HttpRequestMessage original, CancellationToken ct)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri)
        {
            Version = original.Version,
            VersionPolicy = original.VersionPolicy
        };

        foreach (var option in original.Options)
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);

        foreach (var header in original.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (original.Content is not null)
        {
            var bytes = await original.Content.ReadAsByteArrayAsync(ct);
            clone.Content = new ByteArrayContent(bytes);

            foreach (var header in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }

    private static Task DelayAsync(TimeSpan delay, CancellationToken ct)
    {
        return Task.Delay(delay, ct);
    }
}
