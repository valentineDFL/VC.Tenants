using VC.Tenants.Entities;

namespace VC.Tenants.Application.Models.Create;

public record CreateTenantParams
    (string Name,
     CreateConfigurationDto Config,
     TenantStatus Status,
     CreateContactInfoDto ContactInfo,
     CreateWorkScheduleDto WorkSchedule);