using FluentResults;

namespace VC.Tenants.Application.TenantsUseCases.Interfaces;

public interface IDeleteTenantUseCase
{
    public Task<Result> ExecuteAsync();
}