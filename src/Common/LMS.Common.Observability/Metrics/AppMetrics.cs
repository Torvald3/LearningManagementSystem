using System.Collections.Concurrent;

namespace LMS.Common.Observability.Metrics;

public class AppMetrics
{
    private long _httpRequestsTotal;
    private long _httpRequestsFailedTotal;
    private long _activeUserSessions;

    private readonly ConcurrentDictionary<string, int> _coursesGauge = new();
    private readonly ConcurrentBag<double> _requestDurationsMs = new();

    public long HttpRequestsTotal => _httpRequestsTotal;
    public long HttpRequestsFailedTotal => _httpRequestsFailedTotal;
    public long ActiveUserSessions => _activeUserSessions;
    public int CoursesTotal => _coursesGauge.Count;
    public IReadOnlyCollection<double> RequestDurationsMs => _requestDurationsMs.ToArray();

    public void IncrementHttpRequests() => Interlocked.Increment(ref _httpRequestsTotal);

    public void IncrementFailedRequests() => Interlocked.Increment(ref _httpRequestsFailedTotal);

    public void ObserveRequestDuration(double durationMs) => _requestDurationsMs.Add(durationMs);

    public void SessionOpened() => Interlocked.Increment(ref _activeUserSessions);

    public void SessionClosed()
    {
        if (_activeUserSessions > 0)
            Interlocked.Decrement(ref _activeUserSessions);
    }

    public void CourseCreated(Guid id) => _coursesGauge[id.ToString()] = 1;

    public void CourseDeleted(Guid id) => _coursesGauge.TryRemove(id.ToString(), out _);
    public void Reset()
    {
        Interlocked.Exchange(ref _httpRequestsTotal, 0);
        Interlocked.Exchange(ref _httpRequestsFailedTotal, 0);
        Interlocked.Exchange(ref _activeUserSessions, 0);

        _coursesGauge.Clear();

        while (_requestDurationsMs.TryTake(out _))
        {
        }
    }
}