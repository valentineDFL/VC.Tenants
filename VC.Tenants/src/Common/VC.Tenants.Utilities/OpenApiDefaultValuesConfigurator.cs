using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace VC.Tenants.Utilities;

public class OpenApiDefaultValuesConfigurator : IOpenApiSchemaTransformer
{
    private Dictionary<Type, IOpenApiAny> _typeToDefaultOpenApiValue = new()
    {
        { typeof(string), new OpenApiString("string") },
        { typeof(bool), new OpenApiBoolean(false) },
        { typeof(int), new OpenApiInteger(1) },
        { typeof(Guid), new OpenApiString(Guid.Empty.ToString()) },
        { typeof(DateTime), new OpenApiDateTime(DateTime.UtcNow) },
    };

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        int i = 0;
        foreach (var property in schema.Properties)
        {
            var type = context.JsonTypeInfo.Properties[i].PropertyType;

            SetDefaultValueForProperty(type, property.Value);
            i++;
        }
        return Task.CompletedTask;
    }

    private void SetDefaultValueForProperty(Type type, OpenApiSchema property)
    {
        if (property.Default is not null)
            return;

        if (!_typeToDefaultOpenApiValue.ContainsKey(type))
            return;

        property.Default = _typeToDefaultOpenApiValue[type];
    }
}