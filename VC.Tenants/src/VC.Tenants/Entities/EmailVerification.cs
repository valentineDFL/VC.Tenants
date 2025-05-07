namespace VC.Tenants.Entities;

public class EmailVerification
{
    public const int CodeMinuteValidTime = 8;

    public const int CodeMaxLenght = 10;

    private EmailVerification(Guid tenantId, EmailAddress email, string? code)
    {
        TenantId = tenantId;
        Email = email;
        Code = code;
    }

    public Guid TenantId { get; private set; }

    public EmailAddress Email { get; private set; }

    /// <summary>
    /// Код подтверждения почты
    /// </summary>
    public string? Code { get; private set; }

    public static EmailVerification Create(Guid tenantId, EmailAddress email, string? code)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("Tenant id is empty!");
        if (email is null) throw new ArgumentNullException("Email is null!");

        if (code is null)
            throw new ArgumentNullException("Code is null!");

        if (code.Length == 0 || code.Length > CodeMaxLenght)
            throw new ArgumentException($"Code Length must be higher than 0 and lowest than {CodeMaxLenght + 1} but he {code.Length}");

        return new EmailVerification(tenantId, email, code);
    }
}