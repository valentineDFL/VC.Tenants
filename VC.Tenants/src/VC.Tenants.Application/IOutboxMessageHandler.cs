namespace VC.Tenants.Application;

public interface IOutboxMessageHandler<T>
{
    public Task ExecuteAsync(CancellationToken cts);
}