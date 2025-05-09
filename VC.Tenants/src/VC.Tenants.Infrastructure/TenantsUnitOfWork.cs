﻿using Microsoft.EntityFrameworkCore.Storage;
using VC.Tenants.Infrastructure.Persistence;
using VC.Tenants.Repositories;
using VC.Tenants.UnitOfWork;

namespace VC.Tenants.Infrastructure;

internal class TenantsUnitOfWork : IUnitOfWork
{
    private readonly TenantsDbContext _tenantsDbContext;
    public IDbContextTransaction _transaction;

    private readonly ITenantRepository _tenantRepository;
    private readonly IEmailVerificationRepository _emailVerificationRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;

    public TenantsUnitOfWork(ITenantRepository tenantRepository, TenantsDbContext tenantsDbContext, IEmailVerificationRepository emailVerificationRepository)
    {
        _tenantsDbContext = tenantsDbContext;
        _tenantRepository = tenantRepository;
        _emailVerificationRepository = emailVerificationRepository;
    }

    public ITenantRepository TenantRepository => _tenantRepository;

    public IEmailVerificationRepository EmailVerificationRepository => _emailVerificationRepository;

    public IOutboxMessageRepository OutboxMessageRepository => _outboxMessageRepository;

    public void Dispose()
        => _tenantsDbContext.Dispose();

    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null)
            throw new InvalidOperationException("Transaction already started!");

        _transaction = await _tenantsDbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _tenantsDbContext.SaveChangesAsync();
            await _transaction?.CommitAsync();
        }
        catch (Exception)
        {
            await RollBackAsync();
            throw;
        }
        finally
        {
            if(_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollBackAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction must be in progress");

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}