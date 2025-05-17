using Microsoft.EntityFrameworkCore;
using VC.Tenants.Entities;
using VC.Tenants.Repositories;

namespace VC.Tenants.Infrastructure.Persistence.Repositories;

internal class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly TenantsDbContext _tenantDbContext;

    private const int BatchSize = 100;

    public OutboxMessageRepository(TenantsDbContext dbContext)
    {
        _tenantDbContext = dbContext;
    }

    public async Task<List<OutboxMessage?>> GetUnProcessedMessagesByTypeAsync(string typeFullname)
        => await _tenantDbContext.OutboxMessages.OrderBy(d => d.OccuredOnUtc)
            .Where(m => m.Type == typeFullname)
            .Take(BatchSize)
            .ToListAsync();

    public async Task<OutboxMessage?> GetMessageByIdAsync(Guid id)
    {
        var message = await _tenantDbContext.OutboxMessages
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);

        return message;
    }

    public async Task AddMessageAsync(OutboxMessage message)
    {
        await _tenantDbContext.OutboxMessages.AddAsync(message);
    }

    public Task UpdateMessageAsync(OutboxMessage message)
    {
        _tenantDbContext.OutboxMessages.Update(message);

        return Task.CompletedTask;
    }

    public Task RemoveMessageAsync(OutboxMessage message)
    {
        _tenantDbContext.OutboxMessages.Remove(message);

        return Task.CompletedTask;
    }
}