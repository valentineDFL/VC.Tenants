using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using VC.Shared.Utilities;
using VC.Tenants.Application.Contracts;

namespace VC.Tenants.Infrastructure.Implementations.Rabbit;

internal class DirectRabbitPublisher : IPublisher
{
    private IChannel _channel;

    public DirectRabbitPublisher(IOptions<ConnectionStrings> connectionStrings)
    {
        InitAsync(connectionStrings.Value.RabbitMQ).Wait();
    }

    private async Task InitAsync(string connectionString)
    {
        var connectionFactory = new ConnectionFactory() { HostName = connectionString };

        var connection = await connectionFactory.CreateConnectionAsync();

        _channel = await connection.CreateChannelAsync();
    }

    public async Task SendMessageToExchangeAsync(string exchange ,string routeKey, string data, CancellationToken cts = default)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);

        await _channel.BasicPublishAsync(exchange: exchange, routingKey: routeKey, body: dataBytes);
    }

    public async Task SendMessageToExchangeAsync(string exchange, string routeKey, int data, CancellationToken cts = default)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data.ToString());

        await _channel.BasicPublishAsync(exchange: exchange, routingKey: routeKey, body: dataBytes);
    }

    public async Task SendMessageToExchangeAsync(string exchange, string routeKey, bool data, CancellationToken cts = default)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data.ToString());

        await _channel.BasicPublishAsync(exchange: exchange, routingKey: routeKey, body: dataBytes);
    }
}