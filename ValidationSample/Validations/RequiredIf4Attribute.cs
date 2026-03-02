using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Validations
{
    public class RequiredIf4Attribute : RequiredAttribute
    {
        private readonly string _condition;

        // 1. Otimização: Uso de 'record struct' (ValueType) como chave do dicionário.
        // Isso é alocado na Stack, eliminando a criação de strings concatenadas no Heap.
        private readonly record struct CacheKey(Type Type, string Condition);

        private static readonly ConcurrentDictionary<CacheKey, ScriptRunner<bool>> _compiledScriptsCache = new();

        public RequiredIf4Attribute(string condition)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var instanceType = instance.GetType();

            // Instanciação de struct não aloca memória no Garbage Collector
            var cacheKey = new CacheKey(instanceType, _condition);

            // 2. Otimização: TryGetValue Fast-Path.
            // Evita a chamada de método e preparação do GetOrAdd caso o item já exista.
            if (!_compiledScriptsCache.TryGetValue(cacheKey, out var scriptRunner))
            {
                // 3. Otimização: Uso da palavra-chave 'static' no lambda.
                // Isso força o compilador a garantir que nenhuma variável externa seja capturada,
                // lendo os dados estritamente do parâmetro 'key', evitando a alocação de Closure.
                scriptRunner = _compiledScriptsCache.GetOrAdd(cacheKey, static key =>
                {
                    // 1. Configurando as opções do Script
                    var options = ScriptOptions.Default
                        // Carrega a DLL onde o seu Model (e provavelmente o Enum) está hospedado
                        .WithReferences(key.Type.Assembly);
                        // Opcional: Importa o namespace do model, permitindo escrever a condição sem o caminho completo
                        //.WithImports(key.Type.Namespace, "System");

                    var script = CSharpScript.Create<bool>(
                        key.Condition,
                        options: options,
                        globalsType: key.Type);

                    return script.CreateDelegate();
                });
            }

            var evaluated = scriptRunner(instance)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (evaluated)
            {
                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}