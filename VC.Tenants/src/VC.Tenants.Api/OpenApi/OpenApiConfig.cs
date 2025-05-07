using Microsoft.AspNetCore.OpenApi;
using VC.Utilities;

namespace VC.Tenants.Api.OpenApi;

public class OpenApiConfig
{
    public const string GroupName = "Tenants Module";
    public const string DocumentName = "tenants";
    
    public static OpenApiOptions ConfigureOpenApi(OpenApiOptions opts)
    {
        opts.ShouldInclude = description => description.GroupName == GroupName;
        opts.AddDocumentTransformer((document, ctx, ctl) =>
            {
                document.Info = new()
                {
                    Version = "v1",
                    Title = "Управление арендаторами"
                };

                return Task.CompletedTask;
            }
        );

        opts.AddSchemaTransformer<OpenApiDefaultValuesConfigurator>();

        return opts;
    }
}
