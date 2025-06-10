using FluentResults;
using VC.Tenants.Entities.Tenants;

namespace VC.Tenants.Application.TenantsUseCases.Interfaces;

public interface IGetTenantUseCase
{
    public Task<Result<Tenant>> ExecuteAsync();
}