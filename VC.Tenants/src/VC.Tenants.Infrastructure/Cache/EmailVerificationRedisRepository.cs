using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using VC.Tenants.Entities;
using VC.Tenants.Repositories;
using VC.Tenants.Infrastructure.Persistence;
using VC.Tenants.Dtos;

namespace VC.Tenants.Infrastructure.Cache;

internal class EmailVerificationRedisRepository : IEmailVerificationRepository
{
    private const string EmailVerificationKeyPrefix = "emailVerification:";

    private readonly IDistributedCache _cache;

    public EmailVerificationRedisRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(EmailVerification email)
    {
        var key = GenerateKey(email.TenantId, email.Email.Email);
        var value = JsonSerializer.Serialize(email);

        await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(EmailVerification.CodeMinuteValidTime)
        });
    }

    public async Task<EmailVerification> GetAsync(Guid tenantId, string email)
    {
        var key = GenerateKey(tenantId, email);
        var resultJson = await _cache.GetStringAsync(key);

        if (resultJson == null)
            return null;

        var desirializedDto = JsonSerializer.Deserialize<EmailVerificationDto>(resultJson);

        var emailAddress = EmailAddress.Create(desirializedDto.Email.Email, desirializedDto.Email.IsConfirmed);

        return EmailVerification.Create(desirializedDto.TenantId, emailAddress, desirializedDto.Code);
    }

    public async Task UpdateAsync(EmailVerification email)
    {
        var key = GenerateKey(email.TenantId, email.Email.Email);
        var value = JsonSerializer.Serialize(email);

        await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(EmailVerification.CodeMinuteValidTime)
        });
    }

    public async Task RemoveAsync(EmailVerification email)
    {
        var key = GenerateKey(email.TenantId, email.Email.Email);

        await _cache.RemoveAsync(key);
    }

    private string GenerateKey(Guid tenantId, string email)
        => $"{TenantsDbContext.CacheKeyPrefix}{EmailVerificationKeyPrefix}_{tenantId}_{email}";
}