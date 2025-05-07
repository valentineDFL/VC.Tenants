namespace VC.Tenants.Api.Models.Response;

public record ResponseContactInfo
    (string Phone, 
     ResponseAddressDto Address,
     ResponseEmailAddressDto EmailAddress);