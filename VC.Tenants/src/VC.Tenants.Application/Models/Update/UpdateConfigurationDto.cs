namespace VC.Tenants.Application.Models.Update;

public record UpdateConfigurationDto
    (string About,
     string Currency, 
     string Language, 
     string TimeZoneId);