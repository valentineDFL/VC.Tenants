using FluentResults;

namespace VC.Tenants.Application.TenantsUseCases.Interfaces;

public interface IGetTenantIdByUserIdUseCase
{
    public Task<Result<Guid>> ExecuteAsync(Guid userId);
}