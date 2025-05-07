using VC.Tenants.Entities;

namespace VC.Tenants.Api.Models.Request.Update;

public record UpdateTenantRequest
    (string Name,
     UpdateConfigurationDto Config,
     TenantStatus Status,
     UpdateContactInfoDto ContactInfo,
     UpdateWorkScheduleDto WorkSchedule);