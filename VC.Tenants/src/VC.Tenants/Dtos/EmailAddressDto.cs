namespace VC.Tenants.Dtos;

public class EmailAddressDto
{
    public EmailAddressDto(string email, bool isConfirmed)
    {
        Email = email;
        IsConfirmed = isConfirmed;
    }

    public string Email { get; private set; }

    public bool IsConfirmed { get; private set; }
}