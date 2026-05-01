namespace EToro.Configuration;

public sealed class RateLimitOptions
{
    public const int DefaultPermitLimit = 60;
    public static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan DefaultRetryDelayValue = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan DefaultMaxRetryDelay = TimeSpan.FromSeconds(60);

    public bool Enabled { get; set; } = true;
    public int PermitLimit { get; set; } = DefaultPermitLimit;
    public TimeSpan Window { get; set; } = DefaultWindow;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan DefaultRetryDelay { get; set; } = DefaultRetryDelayValue;
    public TimeSpan MaxRetryDelay { get; set; } = DefaultMaxRetryDelay;
    public double RetryJitterRatio { get; set; } = 0.1;
    public bool RetryNonIdempotentRequests { get; set; }
}
