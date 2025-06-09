using VC.Tenants.Entities.Tenants;

namespace VC.Tenants.Repositories;

public interface ITenantRepository
{
    public Task<Tenant?> GetAsync();

    public Task<Guid?> GetByUserIdAsync(Guid userId);

    public Task AddAsync(Tenant tenant);

    public Task RemoveAsync(Tenant tenant);

    public Task UpdateAsync(Tenant tenant);
}