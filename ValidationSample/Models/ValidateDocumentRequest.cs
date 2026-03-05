using ValidationSample.OpenApi;

namespace ValidationSample.Models
{
    public class ValidateDocumentRequest : ValidateDocumentRequestBase
    {   
        //public override string? DocumentNumber { get; set; }
    }

    public class ValidateDocumentRequestBase
    {
        [OpenApiRequired]
        public DocumentType DocumentType { get; set; }

        [OpenApiRequired]
        public virtual string? DocumentNumber { get; set; }
    }
}
