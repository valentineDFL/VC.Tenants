using VC.Tenants.Entities;

namespace VC.Tenants.Api.Models.Request.Create;

public record CreateTenantRequest
    (string Name,
     CreateConfigurationDto Config,
     TenantStatus Status,
     CreateContactInfoDto ContactInfo,
     CreateWorkScheduleDto WorkSchedule);