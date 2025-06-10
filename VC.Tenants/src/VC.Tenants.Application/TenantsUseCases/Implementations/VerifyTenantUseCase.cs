using FluentResults;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.Entities.Tenants.ContactInfos;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Application.TenantsUseCases.Implementations;

internal class VerifyTenantUseCase : IVerifyTenantEmailUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public VerifyTenantUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(string code)
    {
        var tenant = await _unitOfWork.TenantRepository.GetAsync();

        if (tenant is null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        if (tenant.ContactInfo.EmailAddress.IsConfirmed)
            return Result.Fail(ErrorMessages.TenantHasAlreadyBeenVerified);

        var emailVerification = await _unitOfWork.EmailVerificationRepository
            .GetAsync(tenant.Id, tenant.ContactInfo.EmailAddress.Email);

        if (emailVerification is null)
            return Result.Fail(ErrorMessages.ConfirmationTimeHasExpired);

        if (emailVerification.Code != code)
            return Result.Fail(ErrorMessages.CodesDoesNotEquals);

        var emailAddres = tenant.ContactInfo.EmailAddress;
        var updatedEmailAddress = EmailAddress.Create(emailAddres.Email, true);
        var updatedContactInfo = ContactInfo.Create(tenant.ContactInfo.Phone, tenant.ContactInfo.Address, updatedEmailAddress);

        tenant.Update(tenant.Config, tenant.Status, updatedContactInfo, tenant.WorkSchedule);

        await _unitOfWork.TenantRepository.UpdateAsync(tenant);

        await _unitOfWork.CommitAsync();

        return Result.Ok();
    }
}