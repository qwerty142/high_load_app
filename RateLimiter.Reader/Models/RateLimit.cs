namespace RateLimiter.Reader.Models;

public class RateLimit
{
    public RateLimit(string route, int limit)
    {
        Route = route;
        Limit = limit;
    }

    public string Route { get; set; }
    public int Limit { get; set; }
}