using VC.Tenants.Entities;

namespace VC.Tenants.Repositories;

public interface IEmailVerificationRepository
{
    public Task AddAsync(EmailVerification email);

    public Task<EmailVerification> GetAsync(Guid tenantId, string email);

    public Task UpdateAsync(EmailVerification email);

    public Task RemoveAsync(EmailVerification email);
}