﻿using VC.Tenants.Entities.Tenants;

namespace VC.Tenants.Api.Models.Response;

public record ResponseTenantDto
    (string Name,
     string Slug,
     ResponseConfigurationDto Config,
     TenantStatus Status,
     ResponseContactInfo ContactInfo,
     ResponseWorkScheduleDto WorkSchedule);