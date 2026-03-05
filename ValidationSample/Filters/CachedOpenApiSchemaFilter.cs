using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ValidationSample.Filters
{
    public class CachedOpenApiSchemaFilter : IOpenApiSchemaTransformer
    {
        // 2. Um Record interno para guardar apenas o que importa (Nome no JSON e os Modificadores)
        private sealed record CachedProperty(string JsonName, IOpenApiPropertyModifier[] Modifiers);

        // 1. O Cache Estático: Garante que os dados fiquem na memória durante a vida da aplicação
        private static readonly ConcurrentDictionary<Type, CachedProperty[]> _cache = new();

        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            var modelType = context.JsonTypeInfo?.Type;

            if (schema.Properties is null || modelType is null)
                return Task.CompletedTask;

            // 3. Busca no cache. Se for a primeira vez, executa a Reflection e guarda o resultado.
            var cachedProperties = _cache.GetOrAdd(modelType, type =>
            {
                var properties = type.GetProperties();
                var validProperties = new List<CachedProperty>();

                foreach (var property in properties)
                {
                    var modifiers = property.GetCustomAttributes(typeof(IOpenApiPropertyModifier), false);

                    if (modifiers.Length == 0)
                        continue;

                    // Faz a conversão de string e o cast pesado APENAS UMA VEZ por propriedade
                    var jsonName = JsonNamingPolicy.CamelCase.ConvertName(property.Name);
                    var castedModifiers = modifiers.Cast<IOpenApiPropertyModifier>().ToArray();

                    validProperties.Add(new CachedProperty(jsonName, castedModifiers));
                }

                return validProperties.ToArray();
            });

            // 4. Loop de alta performance: usa apenas os dados já cacheados
            foreach (var cachedProp in cachedProperties)
            {
                if (!schema.Properties.TryGetValue(cachedProp.JsonName, out var propertySchemaInterface) ||
                    propertySchemaInterface is not OpenApiSchema propertySchema)
                {
                    continue;
                }

                foreach (var modifier in cachedProp.Modifiers)
                {
                    modifier.ApplyModifier(schema, propertySchema, cachedProp.JsonName);
                }
            }

            return Task.CompletedTask;
        }
    }
}
