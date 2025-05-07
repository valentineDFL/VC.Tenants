namespace VC.Tenants.Application.Contracts;

public interface IEmailVerifyCodeGenerator
{
    public string GenerateCode();
}