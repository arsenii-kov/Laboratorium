namespace WebApp.Middleware;

public class RequestStats
{
    private long _total;

    public long Total
    {
        get { return Interlocked.Read(ref _total); }
    }

    public long Hit()
    {
        return Interlocked.Increment(ref _total);
    }
}
