namespace VC.Tenants.Application.Contracts;

public interface IPublisher
{
    public Task SendMessageToExchangeAsync(string exchange, string routeKey, string data, CancellationToken cts = default);
    public Task SendMessageToExchangeAsync(string exchange, string routeKey, int data, CancellationToken cts = default);
    public Task SendMessageToExchangeAsync(string exchange, string routeKey, bool data, CancellationToken cts = default);
}