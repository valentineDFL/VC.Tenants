using Microsoft.AspNetCore.Http;

namespace VC.Tenants.Utilities.Resolvers;

public class HttpContextTenantResolver : ITenantResolver
{
    private readonly IHttpContextAccessor _context;

    public HttpContextTenantResolver(IHttpContextAccessor context)
    {
        _context = context;
    }

    public Guid Resolve()
    {
        // заглушка до времён реализации аутентификации через JWT + Cookie

        return TenantsIds.StaticTenantId;
    }
}