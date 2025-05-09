using System.Diagnostics;
using VC.Tenants.Application;

namespace VC.Tenants.Host.Background_Services;

internal class OutboxBackgroundService : BackgroundService
{
    private const int SecondsFrequency = 3;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OutboxBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sw = new Stopwatch();

        try
        {
            var seconds = 0.0;
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();

                await handler.ExecuteAsync(stoppingToken);

                sw.Start();

                while(seconds < SecondsFrequency)
                    seconds = sw.Elapsed.TotalSeconds;

                seconds = 0;
                sw.Reset();
            }
        }
        catch (Exception)
        {
            // logging
        }
    }
}