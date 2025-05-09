using Microsoft.EntityFrameworkCore;
using VC.Tenants.Entities;
using VC.Tenants.Repositories;

namespace VC.Tenants.Infrastructure.Persistence.Repositories;

internal class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly TenantsDbContext _tenantsDbContext;

    public OutboxMessageRepository(TenantsDbContext tenantsDbContext)
    {
        _tenantsDbContext = tenantsDbContext;
    }

    public async Task<List<OutboxMessage?>> GetUnProcessedMessagesAsync()
        => await _tenantsDbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccuredOnUtc)
            .ToListAsync();

    public async Task<OutboxMessage?> GetMessageByIdAsync(Guid id)
        => await _tenantsDbContext.OutboxMessages
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task AddMessageAsync(OutboxMessage message)
        => await _tenantsDbContext.OutboxMessages.AddAsync(message);

    public Task UpdateMessageAsync(OutboxMessage message)
    {
        _tenantsDbContext.Update(message);

        return Task.CompletedTask;
    }

    public Task RemoveMessageAsync(OutboxMessage message)
    {
        _tenantsDbContext.Remove(message);

        return Task.CompletedTask;
    }
}