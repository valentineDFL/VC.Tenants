namespace VC.Tenants.Application;

public interface IMessageHandler<T>
{
    public Task ExecuteAsync(CancellationToken cts);
}