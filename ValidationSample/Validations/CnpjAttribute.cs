using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ValidationSample.Validations
{
    public class CnpjAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // [Required] cuida de campos nulos
            }

            string cnpj = Regex.Replace(value.ToString()!, @"[^\d]", "");

            if (IsValidCnpj(cnpj))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "O CNPJ informado é inválido.");
        }

        private static bool IsValidCnpj(string cnpj)
        {
            if (cnpj.Length != 14) return false;

            // Elimina sequências conhecidas de números repetidos
            if (new string(cnpj[0], 14) == cnpj) return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }
}
