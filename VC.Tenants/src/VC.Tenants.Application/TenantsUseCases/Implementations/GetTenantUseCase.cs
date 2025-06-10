using FluentResults;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.Entities.Tenants;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Application.TenantsUseCases.Implementations;

internal class GetTenantUseCase : IGetTenantUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTenantUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Tenant>> ExecuteAsync()
    {
        var tenant = await _unitOfWork.TenantRepository.GetAsync();

        if (tenant is null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        return Result.Ok(tenant);
    }
}