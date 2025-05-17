using VC.Tenants.Entities;

namespace VC.Tenants.Repositories;

public interface IOutboxMessageRepository
{
    public Task<List<OutboxMessage?>> GetUnProcessedMessagesByTypeAsync(string typeFullname);

    public Task<OutboxMessage?> GetMessageByIdAsync(Guid id);

    public Task AddMessageAsync(OutboxMessage message);

    public Task UpdateMessageAsync(OutboxMessage message);

    public Task RemoveMessageAsync(OutboxMessage message);
}