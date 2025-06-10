using FluentResults;
using System.Text.Json;
using VC.Shared.RabbitMQIntegration.Publishers.Interfaces;
using VC.Shared.Utilities.RabbitEnums;
using VC.Tenants.Application.Contracts;
using VC.Tenants.Application.Models.Create;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.Entities;
using VC.Tenants.Entities.Tenants;
using VC.Tenants.Entities.Tenants.ContactInfos;
using VC.Tenants.Entities.Tenants.Schedule;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Application.TenantsUseCases.Implementations;

internal class CreateTenantUseCase : ICreateTenantUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISlugGenerator _slugGenerator;
    private readonly IEmailVerifyCodeGenerator _emailVerifyCodeGenerator;

    private readonly ITenantEmailVerificationMessagesFactory _formFactory;

    private readonly IPublisher _publisher;

    public CreateTenantUseCase(IUnitOfWork unitOfWork, 
                               ISlugGenerator slugGenerator, 
                               IEmailVerifyCodeGenerator emailVerifyCodeGenerator, 
                               ITenantEmailVerificationMessagesFactory formFactory,
                               IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _slugGenerator = slugGenerator;
        _emailVerifyCodeGenerator = emailVerifyCodeGenerator;
        _formFactory = formFactory;
    }

    public async Task<Result<Tenant>> ExecuteAsync(CreateTenantParams @params)
    {
        var tenant = CreateTenantFromParams(@params);

        var code = _emailVerifyCodeGenerator.GenerateCode();

        var emailVerification = EmailVerification.Create(tenant.Id,
                                                         tenant.ContactInfo.EmailAddress,
                                                         code);

        await _unitOfWork.EmailVerificationRepository.AddAsync(emailVerification);

        var message = _formFactory.CreateAfterRegistration(code, tenant.Name, tenant.ContactInfo.EmailAddress.Email);

        var outboxMessage = OutboxMessage.Create(Guid.CreateVersion7(), JsonSerializer.Serialize(message), message.GetType().FullName, DateTime.UtcNow);

        await _unitOfWork.BeginTransactionAsync();

        await _unitOfWork.TenantRepository.AddAsync(tenant);
        await _unitOfWork.OutboxMessageRepository.AddMessageAsync(outboxMessage);

        await _unitOfWork.CommitAsync();

        await _publisher.PublishAsync(tenant, Exchanges.CreatedTenantsDirect, RoutingKeys.CreatedTenantsKey, Queues.CreatedTenants);

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

        return Tenant.Create(tenantId, @params.UserId, @params.Name, slug, config, @params.Status, contactInfo, workShedule);
    }
}