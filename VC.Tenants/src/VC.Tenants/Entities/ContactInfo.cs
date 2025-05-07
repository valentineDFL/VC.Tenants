
namespace VC.Tenants.Entities;

/// <summary>
/// Контактная информация.
/// </summary>
public class ContactInfo : ValueObject
{
    public const int PhoneNumberMinLength = 10;
    public const int PhoneNumberMaxLength = 16;

    private ContactInfo(string phone, Address address, EmailAddress emailAddress)
    {
        Phone = phone;
        Address = address;
        EmailAddress = emailAddress;
    }

    private ContactInfo() { }

    public string Phone { get; private set; }

    public Address Address { get; private set; }

    public EmailAddress EmailAddress { get; private set; }

    public static ContactInfo Create(string phone, Address address, EmailAddress emailAddress)
    {
        if (phone.Length > PhoneNumberMaxLength || phone.Length < PhoneNumberMinLength)
            throw new ArgumentException($"Phone Length must be equals {PhoneNumberMinLength} or {PhoneNumberMaxLength} but he {phone.Length}");

        if(address is null)
            throw new ArgumentNullException("Address cannot be null");

        if (emailAddress is null) throw new ArgumentNullException("Email address cannot be null");

        return new ContactInfo(phone, address, emailAddress);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Phone;
        yield return Address;
        yield return EmailAddress;
    }
}