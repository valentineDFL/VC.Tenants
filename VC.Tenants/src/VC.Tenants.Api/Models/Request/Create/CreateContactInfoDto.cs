namespace VC.Tenants.Api.Models.Request.Create;

public record CreateContactInfoDto
    (string Phone, 
     CreateAddressDto AddressDto, 
     CreateEmailAddressDto EmailAddressDto);