using System.Text.Json;
using VC.Tenants.Application;
using VC.Shared.MailkitIntegration;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Infrastructure.Implementations;

internal class MailSendMessageHandler : IMessageHandler<Message>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMailSender _mailSender;

    public MailSendMessageHandler(IUnitOfWork unitOfWork, IMailSender mailsender)
    {
        _unitOfWork = unitOfWork;
        _mailSender = mailsender;
    }

    public IMailSender Handle => _mailSender;

    public async Task ExecuteAsync(CancellationToken cts)
    {
        var messageType = typeof(Message);

        await _unitOfWork.BeginTransactionAsync();

        var outboxMessages = await _unitOfWork.OutboxMessageRepository.GetUnProcessedMessagesByTypeAsync(messageType.FullName);

        foreach (var outboxMessage in outboxMessages)
        {
            if(cts.IsCancellationRequested)
                break;

            try
            {
                var desirializedContent = JsonSerializer.Deserialize<Message>(outboxMessage!.Content)!;
                
                await _mailSender.SendMailAsync(desirializedContent);

                outboxMessage.Update(processedOnUtc: DateTime.UtcNow);
                await _unitOfWork.OutboxMessageRepository.UpdateMessageAsync(outboxMessage);
            }
            catch(Exception ex)
            {
                outboxMessage!.Update(error: ex.Message);
                await _unitOfWork.OutboxMessageRepository
                        .UpdateMessageAsync(outboxMessage);
            }
        }

        await _unitOfWork.CommitAsync();
    }
}