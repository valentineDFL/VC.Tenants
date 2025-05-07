namespace VC.Tenants.Application.Contracts;

internal interface IPublisher
{
    public Task SendMessageToExchange(string routingKey, string data);
    public Task SendMessageToExchange(string routingKey, int data);
    public Task SendMessageToExchange(string routingKey, bool data);
}