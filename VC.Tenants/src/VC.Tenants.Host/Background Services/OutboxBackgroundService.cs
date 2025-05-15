using System.Diagnostics;
using System.Reflection;

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
        var seconds = 0.0;

        var outboxExtension = new OutboxBackgroundServiceExtension();
        var outboxExtensionType = typeof(OutboxBackgroundServiceExtension);

        var outboxExtensionMethods = outboxExtensionType
            .GetMethods()
            .Where(m => m.ReturnType == typeof(Task))
            .ToList();

        var tasks = new List<Task>();
        var parameters = new object[]
        {
            _serviceScopeFactory,
            stoppingToken
        };

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await HandleAsync(tasks, outboxExtensionMethods, outboxExtension, parameters);

                sw.Start();
                while(seconds < SecondsFrequency)
                    seconds = sw.Elapsed.TotalSeconds;

                seconds = 0;
                sw.Reset();
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    private async Task HandleAsync(List<Task> tasks,
                                   List<MethodInfo> outboxExtensionMethods,
                                   OutboxBackgroundServiceExtension outboxExtension,
                                   object[] parameters)
    {
        foreach (var outboxMethod in outboxExtensionMethods)
        {
            var methodTask = (Task)outboxMethod.Invoke(outboxExtension, parameters);
            tasks.Add(methodTask);
        }

        await Task.WhenAll(tasks);
        tasks.Clear();
    }
}