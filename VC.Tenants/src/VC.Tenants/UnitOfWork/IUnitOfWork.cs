using VC.Tenants.Repositories;

namespace VC.Tenants.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    public ITenantRepository TenantRepository { get; }

    public IEmailVerificationRepository EmailVerificationRepository { get; }

    public IOutboxMessageRepository OutboxMessageRepository { get; }

    public Task BeginTransactionAsync();
    public Task CommitAsync();
    public Task RollBackAsync();
}