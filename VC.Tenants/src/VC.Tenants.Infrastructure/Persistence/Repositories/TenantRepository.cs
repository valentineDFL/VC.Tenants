using Microsoft.EntityFrameworkCore;
using VC.Tenants.Entities;
using VC.Tenants.Repositories;

namespace VC.Tenants.Infrastructure.Persistence.Repositories;

internal class TenantRepository : ITenantRepository
{
    private readonly TenantsDbContext _dbContext;

    public TenantRepository(TenantsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Tenant entity)
        => await _dbContext.Tenants.AddAsync(entity);

    public async Task<Tenant?> GetAsync()
        => await _dbContext.Tenants
            .Include(x => x.WorkSchedule)
            .ThenInclude(x => x.WeekSchedule.OrderBy(d => d.Day))
            .AsNoTracking()
            .FirstOrDefaultAsync();
    
    public Task RemoveAsync(Tenant entity)
    {
        _dbContext.Tenants.Remove(entity);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Tenant entity)
    {
        _dbContext.Tenants.Update(entity);
        return Task.CompletedTask;
    }
}