using FluentResults;
using VC.Tenants.Application.Models.Create;
using VC.Tenants.Application.Models.Update;
using VC.Tenants.Entities;

namespace VC.Tenants.Application.Tenants;

public interface ITenantsService
{
    public Task<Result> CreateAsync(CreateTenantParams @params);

    public Task<Result> DeleteAsync();

    public Task<Result<Tenant>> GetAsync();

    public Task<Result> UpdateAsync(UpdateTenantParams @params);

    public Task<Result> VerifyEmailAsync(string code);

    public Task<Result> SendVerificationMailAsync();
}