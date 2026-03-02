using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Validations
{
    public class ValidateIfAttribute : ValidationAttribute
    {
        private readonly string Condition;
        private readonly ValidationAttribute InnerValidator;

        private readonly record struct ScriptCacheKey(Type Type, string Condition);
        private static readonly ConcurrentDictionary<ScriptCacheKey, ScriptRunner<bool>> CompiledScriptsCache = new();
        private static readonly ConcurrentDictionary<string, ValidationAttribute> ValidatorsCache = new();

        public ValidateIfAttribute(string condition, Type innerValidatorType, params object[] args)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));

            ArgumentNullException.ThrowIfNull(innerValidatorType);

            if (!typeof(ValidationAttribute).IsAssignableFrom(innerValidatorType))
                throw new ArgumentException($"The type {innerValidatorType.Name} must be a ValidationAttribute.");

            var argsKey = args != null && args.Length > 0 ? string.Join("_", args) : "empty";
            var validatorCacheKey = $"{innerValidatorType.FullName}|{argsKey}";

            InnerValidator = ValidatorsCache.GetOrAdd(validatorCacheKey, _ =>
                (ValidationAttribute)Activator.CreateInstance(innerValidatorType, args)!);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var instanceType = instance.GetType();

            var cacheKey = new ScriptCacheKey(instanceType, Condition);

            if (!CompiledScriptsCache.TryGetValue(cacheKey, out var scriptRunner))
            {
                scriptRunner = CompiledScriptsCache.GetOrAdd(cacheKey, static key =>
                {
                    var options = ScriptOptions.Default.WithReferences(key.Type.Assembly);
                    var script = CSharpScript.Create<bool>(key.Condition, options: options, globalsType: key.Type);
                    return script.CreateDelegate();
                });
            }

            var evaluated = scriptRunner(instance)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (evaluated)
                return InnerValidator.GetValidationResult(value, validationContext);

            return ValidationResult.Success;
        }
    }
}