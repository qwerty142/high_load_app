namespace RateLimiter.Reader.Models;

public class RateLimitChange
{
    public RateLimitChange(string route, int limit, ChangeType type)
    {
        Route = route;
        Limit = limit;
        Type = type;
    }

    public string Route { get; set; }
    public int Limit { get; set; }
    public ChangeType Type { get; set; }

    public enum ChangeType
    {
        Insert,
        Update,
        Delete
    }
}