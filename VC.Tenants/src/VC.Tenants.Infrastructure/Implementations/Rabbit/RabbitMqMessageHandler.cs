using System.Text.Json;
using VC.Tenants.Application;
using VC.Tenants.Application.Contracts;
using VC.Tenants.Entities;
using VC.Tenants.Repositories;

namespace VC.Tenants.Infrastructure.Implementations.Rabbit;

internal class RabbitMqMessageHandler : IMessageHandler
{
    private readonly IOutboxMessageRepository _messagesRepository;
    private readonly IPublisher _publisher;

    public RabbitMqMessageHandler(IOutboxMessageRepository messageRepository, IPublisher publisher)
    {
        _messagesRepository = messageRepository;
        _publisher = publisher;
    }

    public async Task ExecuteAsync(CancellationToken cts)
    {
        var messages = await _messagesRepository.GetUnProcessedMessagesAsync();

        OutboxMessage outboxWithException = null;

        foreach (var message in messages)
        {
            try
            {
                var messageType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(t => t.FullName == message.Type)!;

                var deserializedMessage = JsonSerializer.Deserialize(message.Content, messageType)!;

                // call publish message
                await _publisher.SendMessageToExchangeAsync();

                if(deserializedMessage is OutboxMessage instance)
                {
                    outboxWithException = instance;

                    instance.Update(DateTime.UtcNow);

                    await _messagesRepository.UpdateMessageAsync(instance);
                }
            }
            catch(Exception ex) // test way
            {
                outboxWithException.Update(error: ex.Message);
                await _messagesRepository.UpdateMessageAsync(outboxWithException);
            }
        }
    }
}