namespace VC.Tenants.Dtos;

public class EmailVerificationDto
{
    public EmailVerificationDto(Guid tenantId, EmailAddressDto email, string code)
    {
        TenantId = tenantId;
        Email = email;
        Code = code;
    }

    public Guid TenantId { get; private set; }

    public EmailAddressDto Email { get; private set; }

    /// <summary>
    /// Код подтверждения почты
    /// </summary>
    public string Code { get; private set; }
}