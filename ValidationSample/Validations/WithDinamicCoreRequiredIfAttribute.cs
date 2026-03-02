using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ValidationSample.Validations
{
    public class WithDinamicCoreRequiredIfAttribute : RequiredAttribute
    {
        private readonly string _condition;

        // Agora o cache guarda um delegate Func<object, bool> fortemente tipado.
        private static readonly ConcurrentDictionary<string, Func<object, bool>> _compiledExpressionsCache = new();

        public WithDinamicCoreRequiredIfAttribute(string condition)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var instanceType = instance.GetType();

            var cacheKey = $"{instanceType.FullName}|{_condition}";

            var evaluateFunction = _compiledExpressionsCache.GetOrAdd(cacheKey, _ =>
            {
                // 1. Cria o parâmetro representando o modelo tipado (ex: "x" => x.Propriedade)
                var modelParameter = Expression.Parameter(instanceType, "x");

                // 2. Faz o parse da string gerando uma Expression Tree compreensível pelo .NET
                var parsedLambda = DynamicExpressionParser.ParseLambda(
                    new[] { modelParameter },
                    typeof(bool),
                    _condition);

                // 3. Preparando o Wrapper: O ValidationAttribute sempre recebe a instância como 'object'.
                // Precisamos criar um casting nativo na expressão para evitar o uso lento do Delegate.DynamicInvoke().
                var objectParameter = Expression.Parameter(typeof(object), "obj");
                var castExpression = Expression.Convert(objectParameter, instanceType);

                // Invoca a expressão parseada injetando o objeto castado
                var invokeExpression = Expression.Invoke(parsedLambda, castExpression);

                // 4. Compila a árvore final resultando num delegate limpo: (object obj) => parsedLambda((ModelType)obj)
                return Expression.Lambda<Func<object, bool>>(invokeExpression, objectParameter).Compile();
            });

            // 5. Execução limpa, 100% síncrona e com performance comparável a código nativo
            var evaluated = evaluateFunction(instance);

            if (evaluated)
            {
                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}
