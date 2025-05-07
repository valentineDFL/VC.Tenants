namespace VC.Tenants.Application.Models.Create;

public record CreateContactInfoDto
    (string Phone,
     CreateAddressDto AddressDto,
     CreateEmailAddressDto EmailAddressDto);