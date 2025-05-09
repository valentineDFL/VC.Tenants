namespace VC.Tenants.Application;

public interface IMessageHandler
{
    public Task ExecuteAsync(CancellationToken cts);
}