using FluentResults;
using VC.Tenants.Application.Models.Update;
using VC.Tenants.Entities.Tenants;

namespace VC.Tenants.Application.TenantsUseCases.Interfaces;

public interface IUpdateTenantUseCase
{
    public Task<Result<Tenant>> ExecuteAsync(UpdateTenantParams @params);
}