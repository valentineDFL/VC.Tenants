using FluentResults;
using VC.Tenants.Application.Models.Create;
using VC.Tenants.Entities.Tenants;

namespace VC.Tenants.Application.TenantsUseCases.Interfaces;

public interface ICreateTenantUseCase
{
    public Task<Result<Tenant>> ExecuteAsync(CreateTenantParams @params);
}