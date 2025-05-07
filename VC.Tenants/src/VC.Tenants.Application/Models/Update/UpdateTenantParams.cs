using VC.Tenants.Entities;

namespace VC.Tenants.Application.Models.Update;

public record UpdateTenantParams
    (string Name, 
     UpdateConfigurationDto Config, 
     TenantStatus Status,
     UpdateContactInfoDto ContactInfo,
     UpdateWorkScheduleDto WorkSchedule);