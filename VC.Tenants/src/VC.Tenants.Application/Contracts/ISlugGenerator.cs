namespace VC.Tenants.Application.Contracts;

public interface ISlugGenerator
{
    public string GenerateSlug(string name);
}