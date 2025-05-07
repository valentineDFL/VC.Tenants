namespace VC.Tenants.Entities;

public class EmailAddress : ValueObject
{
    public const int EmailAddressMaxLength = 64;

    private EmailAddress(string email, bool isConfirmed)
    {
        Email = email;
        IsConfirmed = isConfirmed;
    }

    private EmailAddress() { }

    public string Email { get; private set; }

    public bool IsConfirmed { get; private set; }

    public static EmailAddress Create(string email, bool isConfirmed = false)
    {
        if (string.IsNullOrEmpty(email)) 
            throw new ArgumentNullException("Email cannot be null or empty");

        if (email.Length > EmailAddressMaxLength)
            throw new ArgumentException($"EmailAddress length must be lowest than {EmailAddressMaxLength} or equals.");

        return new EmailAddress(email, isConfirmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
        yield return IsConfirmed;
    }
}