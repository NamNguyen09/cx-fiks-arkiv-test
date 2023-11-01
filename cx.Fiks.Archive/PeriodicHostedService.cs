using static System.Net.Mime.MediaTypeNames;
using System;

namespace cx.Fiks.Archive;
public class PeriodicHostedService : BackgroundService
{
    private readonly ILogger<PeriodicHostedService> _logger;
    private int _executionCount;

    public PeriodicHostedService(ILogger<PeriodicHostedService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        // When the timer should have no due-time, then do the work once now.
        await DoWork();

        // Using the PeriodicTimer as it does not block resources
        // The loop shall run while no cancellation of the background service is requested in the CancellationToken
        // and wait for the next tick of the timer
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await DoWork();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to execute PeriodicHostedService with exception message {ex.Message}. Good luck next round!");
            }
        }
    }

    // Could also be a async method, that can be awaited in ExecuteAsync above
    private async Task DoWork()
    {
        await Task.Delay(100);

        int count = Interlocked.Increment(ref _executionCount);

        _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
    }
}