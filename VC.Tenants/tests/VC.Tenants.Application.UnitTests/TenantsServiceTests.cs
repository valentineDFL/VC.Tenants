//using Moq;
//using VC.Tenants.Application.Models.Create;
//using VC.Tenants.Application.Models.Update;
//using VC.Tenants.Entities;
//using VC.Tenants.Repositories;
//using VC.Tenants.UnitOfWork;
//using VC.Tenants.Infrastructure.Persistence;
//using VC.Tenants.Application.Tenants;
//using VC.Tenants.Application.Contracts;
//using VC.Tenants.Application;
//using VC.Shared.MailkitIntegration;
//using System.Text.Json;
//using VC.Tenants.Entities.Tenants;
//using VC.Tenants.Entities.Tenants.Schedule;
//using VC.Tenants.Entities.Tenants.ContactInfos;

//namespace VC.Tenants.Tests.Application.UnitTests;

//public class TenantsServiceTests
//{
//    private Tenant _tenant;
//    private EmailVerification _emailVerification;
//    private Message _message;

//    private CreateTenantParams _createParams;
//    private UpdateTenantParams _updateParams;

//    private OutboxMessage _outboxMessage;

//    private ITenantsService _tenantsService;

//    private Mock<ITenantRepository> _tenantsRepository = new Mock<ITenantRepository>();
//    private Mock<IEmailVerificationRepository> _emailVerificationRepository = new Mock<IEmailVerificationRepository>();
//    private Mock<IOutboxMessageRepository> _outboxMessageRepository = new Mock<IOutboxMessageRepository>();

//    private Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();

//    private Mock<ISlugGenerator> _slugGenerator = new Mock<ISlugGenerator>();

//    private Mock<IEmailVerifyCodeGenerator> _codeGenerator = new Mock<IEmailVerifyCodeGenerator>();

//    private Mock<ITenantEmailVerificationMessagesFactory> _formFactory = new Mock<ITenantEmailVerificationMessagesFactory>();

//    public TenantsServiceTests()
//    {
//        _tenant = CreateTenant();
//        _createParams = CreateCreateParams();
//        _emailVerification = CreateEmailVerification();
//        _message = CreateMessage();
//        _updateParams = CreateUpdateParams();
//        _outboxMessage = CreateOutboxMessage();

//        InitTenantsRepository();
//        InitEmailVerificationRepository();
//        InitUnitOfWork();
//        InitMessageFactory();
//        InitOthers();
//        InitOutboxMessageRepository();

//        _tenantsService = new TenantsService(_unitOfWork.Object,
//                                             _slugGenerator.Object,
//                                             _codeGenerator.Object,
//                                             _formFactory.Object);
//    }

//    [Fact]
//    public async Task GetAsync_TenantFound_ReturnsResultOk()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(_tenant);

//        var result = await _tenantsService.GetAsync();

//        Assert.True(result.IsSuccess);
//        Assert.NotNull(result.Value);
//    }

//    [Fact]
//    public async Task GetAsync_TenantNotFound_ReturnsTenantNotFoundError()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(value: null);

//        var result = await _tenantsService.GetAsync();

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.TenantNotFound));
//    }

//    [Fact]
//    public async Task CreateAsync_TenantCreated_ReturnsResultOk()
//    {
//        var result = await _tenantsService.CreateAsync(_createParams);

//        Assert.True(result.IsSuccess);
//    }

//    [Fact]
//    public async Task UpdateAsync_Updated_ReturnsResultOk()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(_tenant);

//        var result = await _tenantsService.UpdateAsync(_updateParams);

//        Assert.True(result.IsSuccess);
//    }

//    [Fact]
//    public async Task UpdateAsync_TenantNotFound_ReturnsTenantNotFoundError()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(value: null);

//        var result = await _tenantsService.UpdateAsync(_updateParams);

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.TenantNotFound));
//    }

//    [Fact]
//    public async Task DeleteAsync_TenantDeleted_ReturnsResultOk()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(_tenant);

//        var result = await _tenantsService.DeleteAsync();

//        Assert.True(result.IsSuccess);
//    }

//    [Fact]
//    public async Task DeleteAsync_TenantNotFound_ReturnsTenantNotFoundError()
//    {
//        _tenantsRepository.Setup(x => x.RemoveAsync(_tenant));

//        var result = await _tenantsService.DeleteAsync();

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.TenantNotFound));
//    }

//    [Fact]
//    public async Task VerifyEmailAsync_TenantNotFound_ReturnsTenantNotFoundError()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(value: null);

//        var result = await _tenantsService.VerifyEmailAsync(_emailVerification.Code);

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.TenantNotFound));
//    }

//    [Fact]
//    public async Task VerifyEmailAsync_TenantHasAlreadyBeenVerified_ReturnsTenantHasAlreadyBeenVerifiedError()
//    {
//        var emailAddress = EmailAddress.Create(_emailVerification.Email.Email, true);
        
//        var contactInfo = ContactInfo.Create(_tenant.ContactInfo.Phone, _tenant.ContactInfo.Address, emailAddress);

//        var tenant = Tenant.Create(_tenant.Id, 
//                                   _tenant.Name, 
//                                   _tenant.Slug, 
//                                   _tenant.Config, 
//                                   _tenant.Status, 
//                                   contactInfo, 
//                                   _tenant.WorkSchedule);

//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(tenant);

//        var result = await _tenantsService.VerifyEmailAsync(_emailVerification.Code);

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.TenantHasAlreadyBeenVerified));
//    }

//    [Fact]
//    public async Task VerifyEmailAsync_ConfirmationTimeHasExpired_ReturnsConfirmationTimeHasExpiredError()
//    {
//        _emailVerificationRepository.Setup(x => x.GetAsync(_tenant.Id, _tenant.ContactInfo.EmailAddress.Email))
//            .ReturnsAsync(value: null);

//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(_tenant);

//        var result = await _tenantsService.VerifyEmailAsync(_emailVerification.Code);

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.ConfirmationTimeHasExpired));
//    }

//    [Fact]
//    public async Task VerifyEmailAsync_CodesDoesNotEquals_ReturnsCodesDoesNotEqualsError()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(_tenant);

//        var wrongCode = "0987654321";
//        var emailVerification = EmailVerification.Create(_tenant.Id, _emailVerification.Email, wrongCode);

//        _emailVerificationRepository.Setup(x => x.GetAsync(_tenant.Id, _tenant.ContactInfo.EmailAddress.Email))
//            .ReturnsAsync(emailVerification);

//        var result = await _tenantsService.VerifyEmailAsync(_emailVerification.Code);

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.CodesDoesNotEquals));
//    }

//    [Fact]
//    public async Task VerifyEmailAsync_Verified_ReturnsResultOk()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(_tenant);

//        _emailVerificationRepository.Setup(x => x.GetAsync(_tenant.Id, _tenant.ContactInfo.EmailAddress.Email))
//            .ReturnsAsync(_emailVerification);

//        var result = await _tenantsService.VerifyEmailAsync(_emailVerification.Code);

//        Assert.True(result.IsSuccess);
//    }

//    [Fact]
//    public async Task SendVerificationMailAsync_TenantNotFound_ReturnsTenantNotFoundError()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(value: null);

//        var result = await _tenantsService.SendVerificationMailAsync();

//        Assert.True(!result.IsSuccess);
//        Assert.True(result.Errors.Any(x => x.Message == ErrorMessages.TenantNotFound));
//    }

//    [Fact]
//    public async Task SendVerificationMailAsync_MailSent_ReturnsResultOk()
//    {
//        _tenantsRepository.Setup(x => x.GetAsync())
//            .ReturnsAsync(_tenant);
        
//        var result = await _tenantsService.SendVerificationMailAsync();

//        Assert.True(result.IsSuccess);
//    }

//    private void InitUnitOfWork()
//    {
//        _unitOfWork.Setup(x => x.Dispose());

//        _unitOfWork.Setup(x => x.BeginTransactionAsync());

//        _unitOfWork.Setup(x => x.CommitAsync());

//        _unitOfWork.Setup(x => x.RollBackAsync());

//        _unitOfWork.Setup(x => x.TenantRepository)
//            .Returns(_tenantsRepository.Object);

//        _unitOfWork.Setup(x => x.EmailVerificationRepository)
//            .Returns(_emailVerificationRepository.Object);

//        _unitOfWork.Setup(x => x.OutboxMessageRepository)
//            .Returns(_outboxMessageRepository.Object);
//    }

//    private void InitTenantsRepository()
//    {
//        _tenantsRepository.Setup(x => x.AddAsync(_tenant));

//        _tenantsRepository.Setup(x => x.UpdateAsync(_tenant));
//    }

//    private void InitEmailVerificationRepository()
//    {
//        _emailVerificationRepository.Setup(x => x.AddAsync(_emailVerification));

//        _emailVerificationRepository.Setup(x => x.GetAsync(_tenant.Id, _tenant.ContactInfo.EmailAddress.Email))
//            .Returns(Task.FromResult(_emailVerification));

//        _emailVerificationRepository.Setup(x => x.UpdateAsync(_emailVerification));

//        _emailVerificationRepository.Setup(x => x.RemoveAsync(_emailVerification));
//    }

//    private void InitOutboxMessageRepository()
//    {
//        _outboxMessageRepository.Setup(x => x.AddMessageAsync(_outboxMessage))
//            .Returns(Task.CompletedTask);
//    }

//    private void InitMessageFactory()
//    {
//        _formFactory.Setup(x => x.CreateAfterRegistration(_emailVerification.Code, _tenant.Name, _tenant.ContactInfo.EmailAddress.Email))
//            .Returns(_message);

//        _formFactory.Setup(x => x.CreateMessageForVerify(_emailVerification.Code, _tenant.Name, _tenant.ContactInfo.EmailAddress.Email))
//            .Returns(_message);
//    }

//    private void InitOthers()
//    {
//        _slugGenerator.Setup(x => x.GenerateSlug(_tenant.Name))
//            .Returns(_tenant.Slug);

//        _codeGenerator.Setup(x => x.GenerateCode())
//            .Returns(_emailVerification.Code);
//    }

//    private Tenant CreateTenant()
//    {
//        var tenantId = Guid.CreateVersion7();

//        var config = TenantConfiguration.Create
//            (
//                "testtesttestdwihdwd",
//                "USD",
//                "RU",
//                "UTC"
//            );

//        var address = Address.Create
//            (
//                "Ukraine",
//                "Kiev",
//                "Pushkina Street",
//                456
//            );

//        var emailAddress = EmailAddress.Create("v.clients.company@gmail.com", false);

//        var contactInfo = ContactInfo.Create
//            (
//                "+123456789",
//                address,
//                emailAddress
//            );

//        var weekSchedule = new List<DaySchedule>
//        {
//            DaySchedule.Create
//            (
//                Guid.CreateVersion7(),
//                tenantId,
//                DayOfWeek.Sunday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            DaySchedule.Create
//            (
//                Guid.CreateVersion7(),
//                tenantId,
//                DayOfWeek.Monday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            DaySchedule.Create
//            (
//                Guid.CreateVersion7(),
//                tenantId,
//                DayOfWeek.Tuesday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            DaySchedule.Create
//            (
//                Guid.CreateVersion7(),
//                tenantId,
//                DayOfWeek.Wednesday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            DaySchedule.Create
//            (
//                Guid.CreateVersion7(),
//                tenantId,
//                DayOfWeek.Thursday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            DaySchedule.Create
//            (
//                Guid.CreateVersion7(),
//                tenantId,
//                DayOfWeek.Friday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            DaySchedule.Create
//            (
//                Guid.CreateVersion7(),
//                tenantId,
//                DayOfWeek.Saturday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            )
//        };

//        var workShedule = WorkSchedule.Create(weekSchedule);

//        return Tenant.Create
//        (
//            tenantId,
//            "AdminTestTenant",
//            SeedingDataBaseKeys.SeedTenantSlug,
//            config,
//            TenantStatus.Active,
//            contactInfo,
//            workShedule
//        );
//    }

//    private CreateTenantParams CreateCreateParams()
//    {
//        var tenantId = Guid.CreateVersion7();

//        var config = new CreateConfigurationDto
//            (
//                "testtesttestdwihdwd",
//                "USD",
//                "RU",
//                "UTC"
//            );

//        var address = new CreateAddressDto
//            (
//                "Ukraine",
//                "Kiev",
//                "Pushkina Street",
//                456
//            );

//        var emailAddress = new CreateEmailAddressDto("v.clients.company@gmail.com");

//        var contactInfo = new CreateContactInfoDto
//            (
//                "+123456789",
//                address,
//                emailAddress
//            );

//        var weekSchedule = new List<CreateDayScheduleDto>
//        {
//            new CreateDayScheduleDto
//            (
//                DayOfWeek.Sunday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new CreateDayScheduleDto
//            (
//                DayOfWeek.Monday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new CreateDayScheduleDto
//            (
//                DayOfWeek.Tuesday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new CreateDayScheduleDto
//            (
//                DayOfWeek.Wednesday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new CreateDayScheduleDto
//            (
//                DayOfWeek.Thursday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new CreateDayScheduleDto
//            (
//                DayOfWeek.Friday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new CreateDayScheduleDto
//            (
//                DayOfWeek.Saturday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            )
//        };

//        var workShedule = new CreateWorkScheduleDto(weekSchedule);

//        return new CreateTenantParams
//        (
//            "AdminTestTenant",
//            config,
//            TenantStatus.Active,
//            contactInfo,
//            workShedule
//        );
//    }

//    private UpdateTenantParams CreateUpdateParams()
//    {
//        var tenantId = Guid.CreateVersion7();

//        var config = new UpdateConfigurationDto
//            (
//                "testtesttestdwihdwd",
//                "USD",
//                "RU",
//                "UTC"
//            );

//        var address = new UpdateAddressDto
//            (
//                "Ukraine",
//                "Kiev",
//                "Pushkina Street",
//                456
//            );

//        var emailAddress = new UpdateEmailAddressDto("v.clients.company@gmail.com");

//        var contactInfo = new UpdateContactInfoDto
//            (
//                "+123456789",
//                address,
//                emailAddress
//            );

//        var days = _tenant.WorkSchedule.WeekSchedule.OrderBy(x => x.Day).ToArray();

//        var weekSchedule = new List<UpdateScheduleDayDto>
//        {
//            new UpdateScheduleDayDto
//            (
//                days[0].Id,
//                DayOfWeek.Sunday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new UpdateScheduleDayDto
//            (
//                days[1].Id,
//                DayOfWeek.Monday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new UpdateScheduleDayDto
//            (
//                days[2].Id,
//                DayOfWeek.Tuesday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new UpdateScheduleDayDto
//            (
//                days[3].Id,
//                DayOfWeek.Wednesday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new UpdateScheduleDayDto
//            (
//                days[4].Id,
//                DayOfWeek.Thursday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new UpdateScheduleDayDto
//            (
//                days[5].Id,
//                DayOfWeek.Friday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            ),
//            new UpdateScheduleDayDto
//            (
//                days[6].Id,
//                DayOfWeek.Saturday,
//                DateTime.UtcNow,
//                DateTime.UtcNow.AddHours(8)
//            )
//        };

//        var workShedule = new UpdateWorkScheduleDto(weekSchedule);

//        return new UpdateTenantParams
//        (
//            "AdminTestTenant",
//            config,
//            TenantStatus.Active,
//            contactInfo,
//            workShedule
//        );
//    }

//    private EmailVerification CreateEmailVerification()
//    {
//        var emailAddress = EmailAddress.Create(_tenant.ContactInfo.EmailAddress.Email, false);

//        return EmailVerification.Create(_tenant.Id, emailAddress, "1234567890");
//    }

//    private Message CreateMessage()
//    {
//        var subject = "Подтверждение почты";
//        var header = "Вы не подтвердили почту!";

//        var text = $"Код подтверждения почты: {_emailVerification.Code}";

//        return new Message(subject, text, _tenant.Name, _tenant.ContactInfo.EmailAddress.Email, header);
//    }

//    private OutboxMessage CreateOutboxMessage()
//    {
//        var id = Guid.CreateVersion7();
//        var content = JsonSerializer.Serialize(_message);
//        var type = typeof(Message).FullName;
//        var occuredOnUtc = DateTime.UtcNow;

//        return OutboxMessage.Create(id, content, type, occuredOnUtc);
//    }
//}