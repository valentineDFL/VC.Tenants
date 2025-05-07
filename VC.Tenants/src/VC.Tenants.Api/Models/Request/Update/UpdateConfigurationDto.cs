namespace VC.Tenants.Api.Models.Request.Update;

public record UpdateConfigurationDto
    (string About,
     string Currency,
     string Language,
     string TimeZoneId);