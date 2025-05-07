namespace VC.Tenants.Api.Models.Request.Create;

public record CreateAddressDto
    (string Country,
     string City,
     string Street,
     int House);