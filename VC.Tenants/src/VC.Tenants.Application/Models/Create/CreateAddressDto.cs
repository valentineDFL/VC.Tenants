namespace VC.Tenants.Application.Models.Create;

public record CreateAddressDto
    (string Country, 
     string City, 
     string Street, 
     int House);