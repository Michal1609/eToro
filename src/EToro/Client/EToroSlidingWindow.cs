namespace EToro.Client;

internal sealed class EToroSlidingWindow(int permitLimit, TimeSpan window, TimeProvider timeProvider)
{
    private readonly Queue<DateTimeOffset> _requests = new();
    private readonly object _lock = new();

    public async Task WaitAsync(CancellationToken ct)
    {
        while (true)
        {
            var delay = ReserveOrGetDelay();
            if (delay <= TimeSpan.Zero)
                return;

            await Task.Delay(delay, timeProvider, ct);
        }
    }

    private TimeSpan ReserveOrGetDelay()
    {
        lock (_lock)
        {
            var now = timeProvider.GetUtcNow();
            PruneExpired(now);

            if (_requests.Count < permitLimit)
            {
                _requests.Enqueue(now);
                return TimeSpan.Zero;
            }

            var retryAt = _requests.Peek() + window;
            var delay = retryAt - now;
            return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
        }
    }

    private void PruneExpired(DateTimeOffset now)
    {
        while (_requests.Count > 0 && now - _requests.Peek() >= window)
            _requests.Dequeue();
    }
}
