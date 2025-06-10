using FluentResults;

namespace VC.Tenants.Application.TenantsUseCases.Interfaces;

public interface ISendVerificationMailUseCase
{
    public Task<Result> ExecuteAsync();
}