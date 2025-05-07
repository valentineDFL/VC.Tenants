namespace VC.Tenants.Api.Models.Response;

public record ResponseAddressDto
    (string Country,
     string City,
     string Street,
     int House);