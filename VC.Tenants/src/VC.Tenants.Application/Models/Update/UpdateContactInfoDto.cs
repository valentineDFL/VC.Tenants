namespace VC.Tenants.Application.Models.Update;

public record UpdateContactInfoDto
    (string Phone, 
     UpdateAddressDto AddressDto, 
     UpdateEmailAddressDto UpdateEmailAddressDto);