namespace VC.Tenants.Api.Models.Request.Update;

public record UpdateAddressDto
    (string Country,
     string City,
     string Street,
     int House);