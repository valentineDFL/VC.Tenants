using VC.Tenants.Entities;

namespace VC.Tenants.Repositories;

public interface ITenantRepository
{
    public Task<Tenant?> GetAsync();

    public Task AddAsync(Tenant tenant);

    public Task RemoveAsync(Tenant tenant);

    public Task UpdateAsync(Tenant tenant);
}