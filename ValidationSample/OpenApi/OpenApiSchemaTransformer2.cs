using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ValidationSample.OpenApi
{
    public class OpenApiSchemaTransformer2 : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            if (schema.Properties is null || context.JsonTypeInfo is null)
                return Task.CompletedTask;

            // 1. Usamos a lista de propriedades já processada e cacheada pelo System.Text.Json
            foreach (var jsonProperty in context.JsonTypeInfo.Properties)
            {
                // 2. Extraímos os atributos polimórficos direto do AttributeProvider
                var attributes = jsonProperty.AttributeProvider?.GetCustomAttributes(typeof(IOpenApiPropertyModifier), false);

                if (attributes is null || attributes.Length == 0)
                    continue;

                // 3. jsonProperty.Name JÁ É o nome correto no JSON (trata JsonPropertyName, camelCase, etc)
                var jsonPropName = jsonProperty.Name;

                if (!schema.Properties.TryGetValue(jsonPropName, out var propertySchemaInterface) ||
                    propertySchemaInterface is not OpenApiSchema propertySchema)
                {
                    continue;
                }

                // 4. Aplica a modificação no schema
                foreach (IOpenApiPropertyModifier modifier in attributes)
                {
                    modifier.ApplyModifier(schema, propertySchema, jsonPropName);
                }
            }

            return Task.CompletedTask;
        }
    }
}