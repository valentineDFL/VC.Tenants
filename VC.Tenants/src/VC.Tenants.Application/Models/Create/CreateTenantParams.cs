using VC.Tenants.Entities.Tenants;

namespace VC.Tenants.Application.Models.Create;

public record CreateTenantParams
    (Guid UserId,
     string Name,
     CreateConfigurationDto Config,
     TenantStatus Status,
     CreateContactInfoDto ContactInfo,
     CreateWorkScheduleDto WorkSchedule);