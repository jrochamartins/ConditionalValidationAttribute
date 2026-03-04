using System.Text.Json.Serialization;

namespace ValidationSample.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<DocumentType>))]
    public enum DocumentType
    {
        //[Description("Outro tipo de documento.")]
        OTHER,
        //[Description("Cadastro de Pessoa Física.")]
        CPF,
        //[Description("Cadastro Nacional de Pessoa Jurídica.")]
        CNPJ
    }
}
