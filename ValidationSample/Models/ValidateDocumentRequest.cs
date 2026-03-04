namespace ValidationSample.Models
{

    /// <summary>
    /// Requisição de Validação de documento
    /// </summary>
    public class ValidateDocumentRequest : ValidateDocumentRequestBase
    {
        [OpenApiRequired]
        public override string? DocumentNumber { get; set; }
    }

    /// <summary>
    /// Requisição de Validação de documento base
    /// </summary>
    public class ValidateDocumentRequestBase
    {
        public DocumentType DocumentType { get; set; }

        public virtual string? DocumentNumber { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OpenApiRequiredAttribute : Attribute { }
}
