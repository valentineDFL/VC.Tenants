namespace VC.Tenants.Application.Models.Create;

public record CreateConfigurationDto
    (string About, 
     string Currency,
     string Language,
     string TimeZoneId);