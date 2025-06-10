using FluentResults;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Application.TenantsUseCases.Implementations;

internal class DeleteTenantUseCase : IDeleteTenantUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTenantUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync()
    {
        var existingTenant = await _unitOfWork.TenantRepository.GetAsync();

        if (existingTenant is null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.TenantRepository.RemoveAsync(existingTenant);
        await _unitOfWork.CommitAsync();

        return Result.Ok();
    }
}