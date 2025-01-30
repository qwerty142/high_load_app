namespace RateLimiter.Reader.Services;

public class BackgroundLimiterLoadService : IHostedService
{
    private readonly ILimiterService _service;

    public BackgroundLimiterLoadService(ILimiterService service)
    {
        _service = service;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _service.InitializeAsync();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}