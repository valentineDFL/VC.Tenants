namespace VC.Tenants.Api.Models.Request.Update;

public record UpdateContactInfoDto
    (string Phone,
     UpdateAddressDto AddressDto,
     UpdateEmailAddressDto UpdateEmailAddressDto);