using FluentResults;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Application.TenantsUseCases.Implementations;

internal class GetTenantIdByUserIdUseCase : IGetTenantIdByUserIdUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTenantIdByUserIdUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> ExecuteAsync(Guid userId)
    {
        var tenantId = await _unitOfWork.TenantRepository.GetIdByUserIdAsync(userId);
        if (tenantId is null)
            return Result.Fail("Not Found");

        return Result.Ok(tenantId.Value);
    }
}