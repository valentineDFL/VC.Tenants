using FluentResults;
using VC.Tenants.Application.Contracts;
using VC.Tenants.Application.Models.Update;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.Entities;
using VC.Tenants.Entities.Tenants;
using VC.Tenants.Entities.Tenants.ContactInfos;
using VC.Tenants.Entities.Tenants.Schedule;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Application.TenantsUseCases.Implementations;

internal class UpdateTenantUseCase : IUpdateTenantUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IEmailVerifyCodeGenerator _emailVerifyCodeGenerator;

    public UpdateTenantUseCase(IUnitOfWork unitOfWork, 
                               IEmailVerifyCodeGenerator emailVerifyCodeGenerator)
    {
        _unitOfWork = unitOfWork;
        _emailVerifyCodeGenerator = emailVerifyCodeGenerator;
    }

    public async Task<Result<Tenant>> ExecuteAsync(UpdateTenantParams @params)
    {
        await _unitOfWork.BeginTransactionAsync();

        var tenant = await _unitOfWork.TenantRepository.GetAsync();

        if (tenant is null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        var tenantParams = CreateTenantParams(@params, tenant);

        var emailAddress = tenantParams.contactInfo.EmailAddress;
        var config = tenantParams.config;
        var contactInfo = tenantParams.contactInfo;
        var workSchedule = tenantParams.schedule;

        if (tenant.ContactInfo.EmailAddress != emailAddress)
        {
            var emailVerification = EmailVerification.Create(tenant.Id,
                emailAddress,
                _emailVerifyCodeGenerator.GenerateCode());

            await _unitOfWork.EmailVerificationRepository.UpdateAsync(emailVerification);
        }

        tenant.Update(config, @params.Status, contactInfo, workSchedule);

        await _unitOfWork.TenantRepository.UpdateAsync(tenant);
        await _unitOfWork.CommitAsync();

        return Result.Ok();
    }

    private (TenantConfiguration config, WorkSchedule schedule, ContactInfo contactInfo) CreateTenantParams(UpdateTenantParams @params, Tenant tenant)
    {
        var config = TenantConfiguration.Create(@params.Config.About, @params.Config.Currency, @params.Config.Language, @params.Config.TimeZoneId);

        var weekSchedule = @params.WorkSchedule.WeekSchedule
            .Select(d => DaySchedule.Create(d.Id, tenant.Id, d.Day, d.StartWork, d.EndWork))
            .OrderBy(d => d.Day).ToList();

        var workSchedule = WorkSchedule.Create(weekSchedule);

        var tenantContactInfo = tenant.ContactInfo.EmailAddress;
        var emailAddress = EmailAddress.Create(@params.ContactInfo.UpdateEmailAddressDto.Email);

        var paramsAddress = @params.ContactInfo.AddressDto;
        var address = Address.Create(paramsAddress.Country, paramsAddress.City, paramsAddress.Street, paramsAddress.House);

        var contactInfo = ContactInfo.Create(@params.ContactInfo.Phone, address, emailAddress);

        return new(config, workSchedule, contactInfo);
    }
}