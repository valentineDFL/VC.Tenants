namespace VC.Tenants.Application.Models.Update;

public record UpdateAddressDto
    (string Country, 
     string City, 
     string Street, 
     int House);