using VC.Tenants.Entities.Tenants;

namespace VC.Tenants.Api.Models.Request.Create;

public record CreateTenantRequest
    (Guid UserId,
     string Name,
     CreateConfigurationDto Config,
     TenantStatus Status,
     CreateContactInfoDto ContactInfo,
     CreateWorkScheduleDto WorkSchedule);