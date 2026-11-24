using System.Collections.Concurrent;

namespace WebApp.Middleware;

public class RequestStats
{
    private long _total;
    private readonly ConcurrentDictionary<string, long> _pathCounts = new ConcurrentDictionary<string, long>();

    public long Total
    {
        get { return Interlocked.Read(ref _total); }
    }

    public long Hit()
    {
        return Interlocked.Increment(ref _total);
    }

    public void RecordPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = "/";
        }

        _pathCounts.AddOrUpdate(path, 1, AddOne);
    }

    private static long AddOne(string key, long currentValue)
    {
        return currentValue + 1;
    }

    public List<KeyValuePair<string, long>> GetTopPaths(int count)
    {
        List<KeyValuePair<string, long>> list = new List<KeyValuePair<string, long>>(_pathCounts);
        list.Sort(CompareByCountDesc);

        List<KeyValuePair<string, long>> result = new List<KeyValuePair<string, long>>();
        int limit = count;
        if (list.Count < limit)
        {
            limit = list.Count;
        }

        for (int i = 0; i < limit; i++)
        {
            result.Add(list[i]);
        }

        return result;
    }

    private static int CompareByCountDesc(KeyValuePair<string, long> a, KeyValuePair<string, long> b)
    {
        return b.Value.CompareTo(a.Value);
    }
}
