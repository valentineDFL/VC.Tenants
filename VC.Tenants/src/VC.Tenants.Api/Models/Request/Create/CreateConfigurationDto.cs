namespace VC.Tenants.Api.Models.Request.Create;

public record CreateConfigurationDto
    (string About,
     string Currency,
     string Language,
     string TimeZoneId);