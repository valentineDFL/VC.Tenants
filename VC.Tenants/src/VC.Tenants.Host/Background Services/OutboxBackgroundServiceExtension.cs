using VC.Shared.MailkitIntegration;
using VC.Tenants.Application;

namespace VC.Tenants.Host.Background_Services;

internal class OutboxBackgroundServiceExtension
{
    public async Task ExecuteEmailMessageHandlerAsync(IServiceScopeFactory serviceScopeFactory, CancellationToken cts = default)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<IOutboxMessageHandler<Message>>();

        await handler.ExecuteAsync(cts);
    }
}