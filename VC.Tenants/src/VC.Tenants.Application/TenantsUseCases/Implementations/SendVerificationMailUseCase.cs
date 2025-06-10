using FluentResults;
using VC.Tenants.Application.Contracts;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.Entities.Tenants.ContactInfos;
using VC.Tenants.Entities;
using VC.Tenants.UnitOfWork;
using System.Text.Json;

namespace VC.Tenants.Application.TenantsUseCases.Implementations;

internal class SendVerificationMailUseCase : ISendVerificationMailUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IEmailVerifyCodeGenerator _emailVerifyCodeGenerator;
    private readonly ITenantEmailVerificationMessagesFactory _formFactory;

    public SendVerificationMailUseCase(IUnitOfWork unitOfWork, 
                                       IEmailVerifyCodeGenerator emailVerifyCodeGenerator, 
                                       ITenantEmailVerificationMessagesFactory formFactory)
    {
        _unitOfWork = unitOfWork;
        _emailVerifyCodeGenerator = emailVerifyCodeGenerator;
        _formFactory = formFactory;
    }

    public async Task<Result> ExecuteAsync()
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

        var message = _formFactory.CreateMessageForVerify(newVerifyCode, tenant.Name, tenant.ContactInfo.EmailAddress.Email);

        var outboxMessage = OutboxMessage.Create(Guid.CreateVersion7(), JsonSerializer.Serialize(message), message.GetType().FullName, DateTime.UtcNow);

        await _unitOfWork.OutboxMessageRepository.AddMessageAsync(outboxMessage);
        await _unitOfWork.CommitAsync();

        return Result.Ok();
    }
}