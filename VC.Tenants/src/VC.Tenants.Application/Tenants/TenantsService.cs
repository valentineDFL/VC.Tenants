using FluentResults;
using VC.Tenants.Application.Contracts;
using VC.Tenants.Application.Models.Create;
using VC.Tenants.Application.Models.Update;
using VC.Tenants.Entities;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Application.Tenants;

internal class TenantsService : ITenantsService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISlugGenerator _slugGenerator;
    private readonly IEmailVerifyCodeGenerator _emailVerifyCodeGenerator;

    //private readonly IMailSender _mailSender;

    private readonly ITenantEmailVerificationMessagesFactory _formFactory;

    public TenantsService(IUnitOfWork unitOfWork,
                          ISlugGenerator slugGenerator,
                          IEmailVerifyCodeGenerator emailVerifyCodeGenerator,
                          //IMailSender mailSender,
                          ITenantEmailVerificationMessagesFactory formFactory)
    {
        _unitOfWork = unitOfWork;
        _slugGenerator = slugGenerator;
        _emailVerifyCodeGenerator = emailVerifyCodeGenerator;
        //_mailSender = mailSender;
        _formFactory = formFactory;
    }

    public async Task<Result<Tenant>> GetAsync()
    {
        var tenant = await _unitOfWork.TenantRepository.GetAsync();

        if (tenant is null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        return Result.Ok(tenant);
    }

    public async Task<Result> CreateAsync(CreateTenantParams @params)
    {
        var tenant = CreateTenantFromParams(@params);

        await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.TenantRepository.AddAsync(tenant);

        var code = _emailVerifyCodeGenerator.GenerateCode();

        var emailVerification = EmailVerification.Create(tenant.Id, 
                                                         tenant.ContactInfo.EmailAddress, 
                                                         code);

        await _unitOfWork.EmailVerificationRepository.AddAsync(emailVerification);
        await _unitOfWork.CommitAsync();

        var message = _formFactory.CreateAfterRegistration(code, tenant.Name, tenant.ContactInfo.EmailAddress.Email);

        //var sendResult = await _mailSender.SendMailAsync(message);

        //if (!sendResult.IsSuccess)
        //    return Result.Fail(sendResult.Errors);

        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(UpdateTenantParams @params)
    {
        await _unitOfWork.BeginTransactionAsync();

        var tenant = await _unitOfWork.TenantRepository.GetAsync();

        if (tenant == null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        var tenantParams = CreateTenantParams(@params, tenant);

        var emailAddress = tenantParams.contactInfo.EmailAddress;
        var config = tenantParams.config;
        var contactInfo = tenantParams.contactInfo;
        var workSchedule = tenantParams.schedule;

        if(tenant.ContactInfo.EmailAddress != emailAddress)
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

    public async Task<Result> DeleteAsync()
    {
        var existingTenant = await _unitOfWork.TenantRepository.GetAsync();

        if (existingTenant is null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        await _unitOfWork.TenantRepository.RemoveAsync(existingTenant);

        await _unitOfWork.CommitAsync();

        return Result.Ok();
    }

    public async Task<Result> VerifyEmailAsync(string code)
    {
        var tenant = await _unitOfWork.TenantRepository.GetAsync();

        if (tenant is null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        if (tenant.ContactInfo.EmailAddress.IsConfirmed)
            return Result.Fail(ErrorMessages.TenantHasAlreadyBeenVerified);

        var emailVerification = await _unitOfWork.EmailVerificationRepository
            .GetAsync(tenant.Id, tenant.ContactInfo.EmailAddress.Email);

        if (emailVerification == null)
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

    public async Task<Result> SendVerificationMailAsync()
    {
        var tenant = await _unitOfWork.TenantRepository.GetAsync();

        if (tenant == null)
            return Result.Fail(ErrorMessages.TenantNotFound);

        var emailAddress = tenant.ContactInfo.EmailAddress;

        var newVerifyCode = _emailVerifyCodeGenerator.GenerateCode();
        var updatedEmailAddress = EmailAddress.Create(emailAddress.Email);

        var updatedContactInfo = ContactInfo.Create(tenant.ContactInfo.Phone, tenant.ContactInfo.Address, updatedEmailAddress);

        tenant.Update(tenant.Config, tenant.Status, updatedContactInfo, tenant.WorkSchedule);

        await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.TenantRepository.UpdateAsync(tenant);

        var emailVerification = EmailVerification.Create(tenant.Id, tenant.ContactInfo.EmailAddress, newVerifyCode);
        await _unitOfWork.EmailVerificationRepository.UpdateAsync(emailVerification);
        await _unitOfWork.CommitAsync();

        //var message = _formFactory.CreateMessageForVerify(newVerifyCode, tenant.Name, tenant.ContactInfo.EmailAddress.Email);
        //var sendMailResult = await _mailSender.SendMailAsync(message);

        //if (!sendMailResult.IsSuccess)
        //    return Result.Fail(sendMailResult.Errors);

        return Result.Ok();
    }

    private Tenant CreateTenantFromParams(CreateTenantParams @params)
    {
        var paramsAddress = @params.ContactInfo.AddressDto;
        var address = Address.Create(paramsAddress.Country, paramsAddress.City, paramsAddress.Street, paramsAddress.House);

        var paramsEmailAddress = @params.ContactInfo.EmailAddressDto;

        var emailAddress = EmailAddress.Create(paramsEmailAddress.Email);

        var contactInfo = ContactInfo.Create(@params.ContactInfo.Phone, address, emailAddress);

        var paramConfig = @params.Config;
        var config = TenantConfiguration.Create(paramConfig.About, paramConfig.Currency, paramConfig.Language, paramConfig.TimeZoneId);

        var tenantId = Guid.CreateVersion7();

        var weekShedule = @params.WorkSchedule.WeekSchedule.Select(x => DaySchedule.Create(Guid.CreateVersion7(), tenantId, x.Day, x.StartWork, x.EndWork)).ToList();
        var workShedule = WorkSchedule.Create(weekShedule);

        var slug = _slugGenerator.GenerateSlug(@params.Name);

        return Tenant.Create(tenantId, @params.Name, slug, config, @params.Status, contactInfo, workShedule);
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

        return new (config, workSchedule, contactInfo);
    }
}