using FluentResults;

namespace VC.Tenants.Application.TenantsUseCases.Interfaces;

public interface IVerifyTenantEmailUseCase
{
    public Task<Result> ExecuteAsync(string code);
}